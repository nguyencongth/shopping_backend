using System.Data;
using Microsoft.Data.SqlClient;
using WebServiceShopping.Models;

namespace WebServiceShopping.Connections
{
    public class ConnectProductType
    {
        public Response productTpyeAll(SqlConnection connection)
        {
            Response response = new Response();
            connection.Open();
            SqlCommand sql = new SqlCommand("sp_loaisp_all", connection);
            sql.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter adapter = new SqlDataAdapter(sql);

            DataTable dataTable = new DataTable();
            
            adapter.Fill(dataTable);
            connection.Close();
            List<ProductType> arrayProductType = new List<ProductType>();
            if (dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    ProductType type = new ProductType();
                    type.categoryId = Convert.ToInt32(dataTable.Rows[i]["categoryId"]);
                    type.categoryName = Convert.ToString(dataTable.Rows[i]["categoryName"]);
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
        public Response getCategoryById(SqlConnection connection, int categoryId)
        {
            Response response = new Response();
            connection.Open();
            SqlCommand cmd = new SqlCommand("getCategoryById", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@categoryId", categoryId);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();

            adapter.Fill(dt);
            connection.Close();

            List<ProductType> categories = new List<ProductType>();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ProductType category = new ProductType();
                    category.categoryId = Convert.ToInt32(dt.Rows[i]["categoryId"]);
                    category.categoryName = Convert.ToString(dt.Rows[i]["categoryName"]);

                    categories.Add(category);
                }
            }

            if (categories.Count > 0)
            {
                response.StatusCode = 200;
                response.StatusMessage = "Thông tin danh mục sản phẩm";
                response.arrayProductType = categories;
            }
            else
            {
                response.StatusCode = 100;
                response.StatusMessage = "Không tìm thấy danh mục sản phẩm!";
                response.arrayProductType = null;
            }
            return response;
        }
        
        public Response createCategory(ProductType category, SqlConnection connection)
        {
            Response response = new Response();
            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("sp_add_new_category", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@categoryName", category.categoryName);
                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "Thêm danh mục sản phẩm thành công!";
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 400;
                response.StatusMessage = "Thêm danh mục sản phẩm thất bại!" + ex.Message;
            }
            finally
            {
                connection.Close();
            }
            return response;
        }
        public Response updateCategory(ProductType category, SqlConnection connection)
        {
            Response response = new Response();
            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("sp_update_category", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@categoryId", category.categoryId);
                cmd.Parameters.AddWithValue("@newCategoryName", category.categoryName);
                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "Cập nhật danh mục sản phẩm thành công!";
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 400;
                response.StatusMessage = "Cập nhật danh mục sản phẩm thất bại!" + ex.Message;
            }
            finally
            {
                connection.Close();
            }
            return response;
        }

        public Response deleteCategory(SqlConnection connection, int categoryId)
        {
            Response response = new Response();
            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("sp_delete_category", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@categoryId", categoryId);
                cmd.ExecuteNonQuery();
                response.StatusCode = 200;
                response.StatusMessage = "Xóa danh mục thành công";
            }
            catch (Exception ex)
            {
                response.StatusCode = 400;
                response.StatusMessage = "Xóa danh mục thất bại! " + ex.Message;
            }
            finally
            {
                connection.Close();
            }
            return response;
        }
    }
}
