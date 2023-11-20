using MySql.Data.MySqlClient;
using System.Data;
using WebServiceShopping.Models;

namespace WebServiceShopping.Connections
{
    public class ConnectProductType
    {
        public Response productTpyeAll(MySqlConnection connection)
        {
            Response response = new Response();
            connection.Open();
            MySqlCommand sql = new MySqlCommand("sp_loaisp_all", connection);
            sql.CommandType = CommandType.StoredProcedure;
            MySqlDataAdapter adapter = new MySqlDataAdapter(sql);

            DataTable dataTable = new DataTable();
            
            adapter.Fill(dataTable);
            connection.Close();
            List<ProductType> arrayProductType = new List<ProductType>();
            if (dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    ProductType type = new ProductType();
                    type.idloaisp = Convert.ToInt32(dataTable.Rows[i]["idloaisp"]);
                    type.tenloaisp = Convert.ToString(dataTable.Rows[i]["tenloaisp"]);
                    arrayProductType.Add(type);
                }
            }
            if (arrayProductType.Count > 0)
            {
                response.StatusCode = 200;
                response.StatusMessage = "Danh sách tất cả loại sản phẩm";
                response.arrayProductType = arrayProductType;
            }
            else
            {
                response.StatusCode = 100;
                response.StatusMessage = "Không tìm được loại sản phẩm nào !";
                response.arrayProductType = null;
            }
            return response;
        }
    }
}
