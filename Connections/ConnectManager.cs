using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using WebServiceShopping.Hash;
using WebServiceShopping.Models;

namespace WebServiceShopping.Connections;

public class ConnectManager
{
    public Response register(Managers managers, SqlConnection connection)
    {
        Response response = new Response();
        connection.Open();

        SqlCommand checkEmailCmd = new SqlCommand("sp_checkEmailAdmin", connection);
        checkEmailCmd.CommandType = CommandType.StoredProcedure;
        checkEmailCmd.Parameters.AddWithValue("@emailValue", managers.email);
        int managerCount = Convert.ToInt32(checkEmailCmd.ExecuteScalar());
        if (managerCount > 0)
        {
            connection.Close();
            response.StatusCode = 100;
            response.StatusMessage = "Email đã tồn tại.";
            return response;
        }

        SqlCommand cmd = new SqlCommand("sp_register_admin", connection);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@full_name", managers.fullName);

        cmd.Parameters.AddWithValue("@email", managers.email);

        cmd.Parameters.AddWithValue("@phone_number", managers.phoneNumber);

        string hashedPassword = PasswordHelper.HashPassword(managers.password);
        cmd.Parameters.AddWithValue("@password_hash", hashedPassword);

        cmd.Parameters.AddWithValue("@roleId", managers.roleId);

        int i = cmd.ExecuteNonQuery();
        connection.Close();
        if (i > 0)
        {
            response.StatusCode = 200;
            response.StatusMessage = "Đăng ký thành công";
        }
        else
        {
            response.StatusCode = 400;
            response.StatusMessage = "Đăng ký thất bại";
        }
        return response;


    }
    
    public Response Login(Login login, SqlConnection connection)
        {
            Response response = new Response();
            connection.Open();
            SqlCommand checkEmailCmd = new SqlCommand("sp_login_admin", connection);
            checkEmailCmd.Parameters.AddWithValue("@email", login.email);
            checkEmailCmd.CommandType = CommandType.StoredProcedure;
            using (SqlDataReader reader = checkEmailCmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    string hashedPasswordFromDb = reader.GetString("password_hash");
                    int idManager = reader.GetInt32("managerId");

                    if (PasswordHelper.VerifyPassword(login.password, hashedPasswordFromDb))
                    {
                        var validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
                        var random = new Random();
                        var secretKey = new string(
                          Enumerable.Repeat(validChars, 64).Select(s => s[random.Next(s.Length)]).ToArray());

                        var tokenHandler = new JwtSecurityTokenHandler();
                        var tokenKey = Encoding.UTF8.GetBytes(secretKey);
                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(new Claim[]
                            {
                                new Claim(ClaimTypes.Name, login.email)
                            }),
                            Expires = DateTime.UtcNow.AddHours(1),
                            SigningCredentials = new SigningCredentials(
                                new SymmetricSecurityKey(tokenKey),
                                SecurityAlgorithms.HmacSha256Signature)

                        };
                        var token = tokenHandler.CreateToken(tokenDescriptor);
                        response.StatusCode = 200;
                        response.StatusMessage = "Đăng nhập thành công.";
                        response.managerId = idManager;
                        response.Token = tokenHandler.WriteToken(token);
                    }
                    else
                    {
                        response.StatusCode = 100;
                        response.StatusMessage = "Mật khẩu không hợp lệ.";
                    }
                    var role = reader["roleName"].ToString();
                    response.role = role;
                }
                else
                {
                    response.StatusCode = 100;
                    response.StatusMessage = "Email không tồn tại.";
                }
            }
            connection.Close();
            return response;
        }

    public Response getStaffAll(SqlConnection connection)
    {
        Response response = new Response();
        connection.Open();
        SqlCommand cmd = new SqlCommand("sp_getStaffAll", connection);
        cmd.CommandType = CommandType.StoredProcedure;
        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        adapter.Fill(dt);
        connection.Close();
        List<Managers> managers = new List<Managers>();
        if (dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Managers manager = new Managers();
                manager.managerId = Convert.ToInt32(dt.Rows[i]["managerId"]);
                manager.fullName = Convert.ToString(dt.Rows[i]["fullName"]);
                manager.email = Convert.ToString(dt.Rows[i]["email"]);
                manager.phoneNumber = Convert.ToString(dt.Rows[i]["phoneNumber"]);
                manager.password = Convert.ToString(dt.Rows[i]["password_hash"]);
                manager.roleId = Convert.ToInt32(dt.Rows[i]["roleId"]);
                //manager.roleName = Convert.ToString(dt.Rows[i]["roleName"]);
                managers.Add(manager);
            }
        }

        if (managers.Count > 0)
        {
            response.StatusCode = 200;
            response.StatusMessage = "Danh sách nhân viên.";
            response.arrayManager = managers;
        }
        else
        {
            response.StatusCode = 400;
            response.StatusMessage = "Không tìm thấy nhân viên nào!";
            response.arrayManager = null;
        }
        return response;
    }

    public Response delStaff(SqlConnection connection, int id)
    {
        Response response = new Response();
        try
        {
            connection.Open();
            SqlCommand cmd = new SqlCommand("sp_del_staff", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@managerId", id);
            cmd.ExecuteNonQuery();
            response.StatusCode = 200;
            response.StatusMessage = "Xóa nhân viên thành công.";
        }
        catch(Exception ex)
        {
            response.StatusCode = 400;
            response.StatusMessage = "Xóa nhân viên thất bại. " + ex.Message;
        }
        finally
        {
            connection.Close();
        }
        
        return response;
    }

    public Response updateInfoStaff(Managers managers, SqlConnection connection)
    {
        Response response = new Response();
        try
        {
            connection.Open();
            SqlCommand cmd = new SqlCommand("sp_update_info_staff", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@fullName", managers.fullName);
            cmd.Parameters.AddWithValue("@email", managers.email);
            cmd.Parameters.AddWithValue("@phoneNumber", managers.phoneNumber);
            cmd.Parameters.AddWithValue("@managerId", managers.managerId);
            cmd.ExecuteNonQuery();
            response.StatusCode = 200;
            response.StatusMessage = "Cập nhật thông tin thành công.";
        }
        catch (Exception ex)
        {
            response.StatusCode = 400;
            response.StatusMessage = "Cập nhật thông tin không thành công." + ex.Message;
        }
        finally
        {
            connection.Close();
        }
        
        return response;
    }

    public Response resetPassword(SqlConnection connection ,int id)
    {
        Response response = new Response();
        try
        {
            connection.Open();
            var passwordReset = "123456";
            string hashedPassword = PasswordHelper.HashPassword(passwordReset);
            SqlCommand cmd = new SqlCommand("sp_reset_password", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@managerId", id);
            cmd.Parameters.AddWithValue("@passwordReset", hashedPassword);
            cmd.ExecuteNonQuery();
            response.StatusCode = 200;
            response.StatusMessage = "Reset mật khẩu thành công.";
        }
        catch (Exception ex)
        {
            response.StatusCode = 400;
            response.StatusMessage = "Reset mật khẩu thất bại." + ex.Message;
        }
        finally
        {
            connection.Close();
        }

        return response;
    }

    public Response getStaffInfoById(SqlConnection connection, int staffId)
    {
        Response response = new Response();
        try
        {
            connection.Open();
            SqlCommand getStaffById = new SqlCommand("sp_getInfoStaffById", connection);
            getStaffById.CommandType = CommandType.StoredProcedure;
            getStaffById.Parameters.AddWithValue("@staffId", staffId);

            SqlDataAdapter adapter = new SqlDataAdapter(getStaffById);

            DataTable dataTable = new DataTable();

            adapter.Fill(dataTable);


            List<Managers> arrayManager = new List<Managers>();

            if (dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Managers manager = new Managers();
                    manager.managerId = Convert.ToInt32(dataTable.Rows[i]["managerId"]);
                    manager.fullName = Convert.ToString(dataTable.Rows[i]["fullName"]);
                    manager.email = Convert.ToString(dataTable.Rows[i]["email"]);
                    manager.phoneNumber = Convert.ToString(dataTable.Rows[i]["phoneNumber"]);
                    manager.password = Convert.ToString(dataTable.Rows[i]["password_hash"]);
                    manager.roleId = Convert.ToInt32(dataTable.Rows[i]["roleId"]);
                    arrayManager.Add(manager);
                }
            }
            if (arrayManager.Count > 0)
            {
                response.StatusCode = 200;
                response.StatusMessage = "Thông tin nhân viên.";
                response.arrayManager = arrayManager;
            }
        }
        catch (Exception ex)
        {
            response.StatusCode = 400;
            response.StatusMessage = "Lấy thông tin nhân viên thất bại." + ex.Message;
        }
        finally
        {
            connection.Close();
        }
        return response;
    }
}