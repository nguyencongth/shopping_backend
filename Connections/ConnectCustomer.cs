
using System.Data;
using MySql.Data.MySqlClient;
using WebServiceShopping.Models;
using WebServiceShopping.Hash;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Net.Mail;
using System.Net;
using Response = WebServiceShopping.Models.Response;

namespace WebServiceShopping.Connections
{
    public class ConnectCustomer
    {
        public Response register(Customers customer, MySqlConnection connection)
        {
            Response response = new Response();
            connection.Open();

            MySqlCommand checkEmailCmd = new MySqlCommand("sp_checkEmail", connection);
            checkEmailCmd.CommandType = CommandType.StoredProcedure;
            checkEmailCmd.Parameters.AddWithValue("emailValue", customer.email);
            int customerCount = Convert.ToInt32(checkEmailCmd.ExecuteScalar());
            if (customerCount > 0)
            {
                connection.Close();
                response.StatusCode = 100;
                response.StatusMessage = "Email đã tồn tại.";
                return response;
            }

            MySqlCommand cmd = new MySqlCommand("sp_register_customer", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("full_name", customer.fullname);

            cmd.Parameters.AddWithValue("emailValue", customer.email);

            cmd.Parameters.AddWithValue("phone_number", customer.phonenumber);

            string hashedPassword = PasswordHelper.HashPassword(customer.password);
            cmd.Parameters.AddWithValue("password_h", hashedPassword);

            cmd.Parameters.AddWithValue("address", customer.address);


            int i = cmd.ExecuteNonQuery();
            connection.Close();
            if (i > 0)
            {
                response.StatusCode = 200;
                response.StatusMessage = "Đăng ký thành công";
            }
            else
            {
                response.StatusCode = 100;
                response.StatusMessage = "Đăng ký thất bại";
            }
            return response;


        }

        public Response Login(Login login, MySqlConnection connection)
        {
            Response response = new Response();
            connection.Open();

            MySqlCommand checkEmailCmd = new MySqlCommand("SELECT id_customer, password_hash FROM customer WHERE email = @Email", connection);
            checkEmailCmd.Parameters.AddWithValue("@Email", login.email);

            using (MySqlDataReader reader = checkEmailCmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    string hashedPasswordFromDb = reader.GetString("password_hash");
                    int idCustomer = reader.GetInt32("id_customer");

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
                        response.id_customer = idCustomer;
                        response.Token = tokenHandler.WriteToken(token);
                    }
                    else
                    {
                        response.StatusCode = 100;
                        response.StatusMessage = "Mật khẩu không hợp lệ.";
                    }
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

        public Response customerAll(MySqlConnection connection)
        {
            Response response = new Response();
            connection.Open();
            MySqlCommand cmd = new MySqlCommand("sp_customer_all", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            connection.Close();
            List<Customers> arrayCustomer = new List<Customers>();
            if (dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Customers customer = new Customers();
                    customer.id_customer = Convert.ToInt32(dataTable.Rows[i]["id_customer"]);
                    customer.fullname = Convert.ToString(dataTable.Rows[i]["fullname"]);
                    customer.email = Convert.ToString(dataTable.Rows[i]["email"]);
                    customer.phonenumber = Convert.ToString(dataTable.Rows[i]["phonenumber"]);
                    customer.password = Convert.ToString(dataTable.Rows[i]["password_hash"]);
                    customer.address = Convert.ToString(dataTable.Rows[i]["address"]);
                    arrayCustomer.Add(customer);
                }
            }
            if (arrayCustomer.Count > 0)
            {
                response.StatusCode = 200;
                response.StatusMessage = "Danh sách tất cả khách hàng";
                response.arrayCustomer = arrayCustomer;
            }
            else
            {
                response.StatusCode = 100;
                response.StatusMessage = "Không tìm được khách hàng nào !";
                response.arrayCustomer = null;
            }
            return response;
        }
        public Response updateInfo(Customers customer, MySqlConnection connection)
        {
            Response response = new Response();
            try
            {
                connection.Open();
                MySqlCommand updateCmd = new MySqlCommand("sp_update_info", connection);
                updateCmd.CommandType = CommandType.StoredProcedure;
                updateCmd.Parameters.AddWithValue("IN_FullName", customer.fullname);
                updateCmd.Parameters.AddWithValue("IN_Email", customer.email);
                updateCmd.Parameters.AddWithValue("IN_PhoneNumber", customer.phonenumber);
                updateCmd.Parameters.AddWithValue("IN_Address", customer.address);
                updateCmd.Parameters.AddWithValue("IN_CustomerID", customer.id_customer);

                int rowsAffected = updateCmd.ExecuteNonQuery();
                connection.Close();
                if (rowsAffected > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "Cập nhật tài khoản thành công.";
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 400;
                response.StatusMessage = "Lỗi khi cập nhật tài khoản." + ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return response;
        }
        public Response changePassword(int customerID, string currentPassword, string newPassword, string confirmNewPassword, MySqlConnection connection)
        {
            Response response = new Response();
            try
            {
                connection.Open();
                MySqlCommand checkPasswordCmd = new MySqlCommand("Select password_hash From customer Where id_customer = @customerID", connection);
                checkPasswordCmd.Parameters.AddWithValue("@customerID", customerID);
                string hashedPasswordFromDb = Convert.ToString(checkPasswordCmd.ExecuteScalar());
                if (PasswordHelper.VerifyPassword(currentPassword, hashedPasswordFromDb))
                {
                    if (newPassword == confirmNewPassword)
                    {
                        string newHashedPassword = PasswordHelper.HashPassword(newPassword);
                        MySqlCommand updatePasswordCmd = new MySqlCommand("Update customer SET password_hash = @NewPassword Where id_customer = @customerID", connection);
                        updatePasswordCmd.Parameters.AddWithValue("@NewPassword", newHashedPassword);
                        updatePasswordCmd.Parameters.AddWithValue("@customerID", customerID);
                        updatePasswordCmd.ExecuteNonQuery();

                        response.StatusCode = 200;
                        response.StatusMessage = "Thay đổi mật khẩu thành công";
                    }
                    else
                    {
                        response.StatusCode = 400;
                        response.StatusMessage = "Mật khẩu xác nhận không chính xác";
                    }
                }
                else
                {
                    response.StatusCode = 401;
                    response.StatusMessage = "Mật khẩu hiện tại không chính xác";
                }

            }
            catch (Exception ex)
            {
                response.StatusCode = 400;
                response.StatusMessage = ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            connection.Close();
            return response;
        }
        public Response getCustomerById(MySqlConnection connection, int CustomerID)
        {
            Response response = new Response();
            try
            {
                connection.Open();
                MySqlCommand getCustomerById = new MySqlCommand("getCustomerById", connection);
                getCustomerById.CommandType = CommandType.StoredProcedure;
                getCustomerById.Parameters.AddWithValue("IN_CustomerID", CustomerID);

                MySqlDataAdapter adapter = new MySqlDataAdapter(getCustomerById);

                DataTable dataTable = new DataTable();

                adapter.Fill(dataTable);


                List<Customers> arrayCustomer = new List<Customers>();

                if (dataTable.Rows.Count > 0)
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        Customers customer = new Customers();
                        customer.id_customer = Convert.ToInt32(dataTable.Rows[i]["id_customer"]);
                        customer.fullname = Convert.ToString(dataTable.Rows[i]["fullname"]);
                        customer.email = Convert.ToString(dataTable.Rows[i]["email"]);
                        customer.phonenumber = Convert.ToString(dataTable.Rows[i]["phonenumber"]);
                        customer.password = Convert.ToString(dataTable.Rows[i]["password_hash"]);
                        customer.address = Convert.ToString(dataTable.Rows[i]["address"]);
                        arrayCustomer.Add(customer);
                    }
                }
                if (arrayCustomer.Count > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "Thông tin người dùng";
                    response.arrayCustomer = arrayCustomer;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 400;
                response.StatusMessage = "Không tìm thấy thông tin người dùng" + ex.Message;
            }
            finally
            {
                connection.Close();
            }
            return response;
        }
        public Response SendPasswordResetOTP(MySqlConnection connection, string email)
        {
            Response response = new Response();
            connection.Open();
            string query = "SELECT * FROM customer WHERE email = @Email";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Email", email);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        response.StatusCode = 400;
                        response.StatusMessage = "Email không tồn tại";
                    }
                    reader.ReadAsync();
                    string userEmail = reader.GetString(reader.GetOrdinal("email"));
                    var otp = new Random().Next(100000, 999999).ToString();
                    var otpExpiry = DateTime.Now.AddMinutes(5);
                    SendOtpToEmailAsync(userEmail, "Your password reset OTP", $"Your OTP is: {otp}");
                    reader.CloseAsync();
                    string updateQuery = "UPDATE customer SET otp = @Otp, otpExpiry = @OtpExpiry WHERE email = @Email";
                    using (MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@Otp", otp);
                        updateCommand.Parameters.AddWithValue("@OtpExpiry", otpExpiry);
                        updateCommand.Parameters.AddWithValue("@Email", userEmail);
                        updateCommand.ExecuteNonQueryAsync();
                    }

                }
            }
            connection.Close();
            response.StatusCode = 200;
            response.StatusMessage = "Gửi otp thành công";
            return response;
        }

        public int SendOtpToEmailAsync(string toEmail, string subject, string body)
        {

            var fromAddress = new MailAddress("thanhnc279@gmail.com", "Thanhnc");
            var toAddress = new MailAddress(toEmail, "To Name");
            const string fromPassword = "yfagiawekhvcuism";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            };
            smtp.Send(message);
            return 0;
        }

        public Response ResetPasswordAsync(MySqlConnection connection, ResetPassword model)
        {
            Response response = new Response();
            connection.Open();
            string query = "SELECT * FROM customer WHERE email = @Email";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Email", model.email);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        response.StatusCode = 400;
                        response.StatusMessage = "Email không tồn tại";
                    }
                    reader.ReadAsync();
                    var userOtp = reader.IsDBNull(reader.GetOrdinal("otp")) ? null : reader.GetString(reader.GetOrdinal("otp"));
                    DateTime? userOtpExpiry = reader.IsDBNull(reader.GetOrdinal("otpExpiry")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("otpExpiry"));
                    string modelOtpAsString = model.otp.ToString();
                    if (userOtp != modelOtpAsString || DateTime.UtcNow > userOtpExpiry)
                    {
                        response.StatusCode = 400;
                        response.StatusMessage = "Lỗi khi xác thực OTP";
                    }
                    string updateQuery = "UPDATE customer SET otp = NULL, otpExpiry = NULL, password_hash = @newPassword WHERE email = @Email";
                    reader.CloseAsync();
                    using (MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@newPassword", PasswordHelper.HashPassword(model.newPassword));
                        updateCommand.Parameters.AddWithValue("@Email", model.email);
                        updateCommand.ExecuteNonQueryAsync();
                    }
                }
            }
            connection.Close();
            response.StatusCode = 200;
            response.StatusMessage = "Đăt lại mật khẩu thành công";
            return response;
        }

    }
}
