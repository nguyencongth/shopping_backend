using MySql.Data.MySqlClient;
using System.Data;
using WebServiceShopping.Models;

namespace WebServiceShopping.Connections
{
    public class ConnectProductType
    {
        // Lấy tất cả loại sản phẩm
        public Response productTpyeAll(MySqlConnection connection)
        {
            Response response = new Response();
            connection.Open();
            MySqlCommand sql = new MySqlCommand("sp_loaisp_all", connection);
            sql.CommandType = CommandType.StoredProcedure;
            MySqlDataAdapter adapter = new MySqlDataAdapter(sql);

            // Sau khi lấy xong dữ liệu ta sẽ tạo ra 1 bảng dữ liệu mới
            DataTable dataTable = new DataTable();
            
            adapter.Fill(dataTable);
            connection.Close();
            // Khởi tạo mảng hứng dữ liệu
            List<ProductType> arrayProductType = new List<ProductType>();
            if (dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    ProductType type = new ProductType();
                    type.idloaisp = Convert.ToInt32(dataTable.Rows[i]["idloaisp"]);
                    type.tenloaisp = Convert.ToString(dataTable.Rows[i]["tenloaisp"]);
                    // Gán dữ liệu vào mảng
                    arrayProductType.Add(type);
                }
            }
            // Kiểm tra nếu mảng có dữ liệu
            if (arrayProductType.Count > 0)
            {
                // Thông báo thành công
                response.StatusCode = 200;
                response.StatusMessage = "Danh sách tất cả loại sản phẩm";
                response.arrayProductType = arrayProductType;
            }
            else
            {
                // Thông báo thất bại
                response.StatusCode = 100;
                response.StatusMessage = "Không tìm được loại sản phẩm nào !";
                response.arrayProductType = null;
            }
            return response;
        }
    }
}
