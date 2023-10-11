
using System.Data;
using MySql.Data.MySqlClient;
using WebServiceShopping.Models;
using WebServiceShopping.Hash;

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
            if(customerCount > 0)
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

            // Kiểm tra xem email có tồn tại trong cơ sở dữ liệu hay không
            MySqlCommand checkEmailCmd = new MySqlCommand("SELECT password_hash FROM customer WHERE email = @Email", connection);
            checkEmailCmd.Parameters.AddWithValue("@Email", login.email);
            using (MySqlDataReader reader = checkEmailCmd.ExecuteReader())
            {
                if(reader.Read())
                {
                    // Lấy mật khẩu từ cơ sở dữ liệu
                    string hashedPasswordFromDb = reader.GetString("password_hash");

                    // Kiểm tra mật khẩu
                    if (PasswordHelper.VerifyPassword(login.password, hashedPasswordFromDb))
                    {
                        // Mật khẩu đúng
                        response.StatusCode = 200;
                        response.StatusMessage = "Đăng nhập thành công.";
                    }
                    else
                    {
                        // Mật khẩu không đúng
                        response.StatusCode = 100;
                        response.StatusMessage = "Mật khẩu không hợp lệ.";
                    }
                }
                else
                {
                    // Email không tồn tại
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
            cmd.CommandType= CommandType.StoredProcedure;
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

            // Sau khi lấy xong dữ liệu ta sẽ tạo ra 1 bảng dữ liệu mới
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            connection.Close();
            // Khởi tạo mảng hứng dữ liệu
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
                    // Gán dữ liệu vào mảng
                    arrayCustomer.Add(customer);
                }
            }
            // Kiểm tra nếu mảng có dữ liệu
            if (arrayCustomer.Count > 0)
            {
                // Thông báo thành công
                response.StatusCode = 200;
                response.StatusMessage = "Danh sách tất cả khách hàng";
                response.arrayCustomer = arrayCustomer;
            }
            else
            {
                // Thông báo thất bại
                response.StatusCode = 100;
                response.StatusMessage = "Không tìm được khách hàng nào !";
                response.arrayCustomer = null;
            }
            return response;
        }
    }
}
