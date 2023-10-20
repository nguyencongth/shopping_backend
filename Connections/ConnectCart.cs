using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Data;
using WebServiceShopping.Models;

namespace WebServiceShopping.Connections
{
    public class ConnectCart
    {
        public Response AddToCart(Cart cart, MySqlConnection connection)
        {
            Response response = new Response();
            MySqlCommand command = new MySqlCommand("sp_add_cart", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("IN_id_customer", cart.id_customer);
            command.Parameters.AddWithValue("IN_idsp", cart.idsp);
            command.Parameters.AddWithValue("IN_quantity", cart.quantity);
            command.Parameters.AddWithValue("IN_dateAdded", DateTime.Now);
            // Mở kết nối
            connection.Open();
            int i = command.ExecuteNonQuery();
            connection.Close();
            if (i > 0)
            {
                response.StatusCode = 200;
                response.StatusMessage = "Thêm sản phẩm vào giỏ hàng thành công";
                return response;
            }
            else
            {
                return null;
            }
        }
        public Response UpdateCartQuantity(Cart cart, MySqlConnection connection)
        {
            Response response = new Response();
            MySqlCommand checkCartCmd = new MySqlCommand("SELECT COUNT(*) FROM cart WHERE id_customer = @customerID AND idsp = @productID", connection);
            checkCartCmd.Parameters.AddWithValue("@customerID", cart.id_customer);
            checkCartCmd.Parameters.AddWithValue("@productID", cart.idsp);

            connection.Open();

            int cartCount = Convert.ToInt32(checkCartCmd.ExecuteScalar());

            if (cartCount > 0)
            {
                // Sản phẩm đã có trong giỏ hàng, cập nhật số lượng
                MySqlCommand updateCmd = new MySqlCommand("UPDATE Cart SET quantity = quantity + @quantity WHERE id_customer = @customerID AND idsp = @productID", connection);
                updateCmd.Parameters.AddWithValue("@quantity", cart.quantity);
                updateCmd.Parameters.AddWithValue("@customerID", cart.id_customer);
                updateCmd.Parameters.AddWithValue("@productID", cart.idsp);
                int rowsUpdated = updateCmd.ExecuteNonQuery();

                if (rowsUpdated > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "Cập nhật số lượng sản phẩm trong giỏ hàng thành công";
                }
                else
                {
                    response.StatusCode = 400;
                    response.StatusMessage = "Lỗi khi cập nhật số lượng sản phẩm trong giỏ hàng";
                }
            }
            else
            {
                response.StatusCode = 400;
                response.StatusMessage = "Sản phẩm chưa có trong giỏ hàng, hãy thêm sản phẩm vào giỏ hàng trước.";
            }

            connection.Close();
            return response;
        }
        public Response deleteCartItem(MySqlConnection connection, int customerID, int productID)
        {
            Response response = new Response();
            MySqlCommand removeCmd = new MySqlCommand("DELETE FROM cart WHERE id_customer = @customerId AND idsp = @productId", connection);
            removeCmd.Parameters.AddWithValue("@customerId", customerID);
            removeCmd.Parameters.AddWithValue("@productID", productID);

            connection.Open();

            int rowsDeleted = removeCmd.ExecuteNonQuery();
            if (rowsDeleted > 0)
            {
                response.StatusCode = 200;
                response.StatusMessage = "Xóa sản phẩm khỏi giỏ hàng thành công.";
                return response;
            }
            else
            {
                response.StatusCode = 400;
                response.StatusMessage = "Lỗi khi xóa sản phẩm khỏi giỏ hàng.";
            }
            connection.Close();
            return response;
        }

        public Response GetCartItemsByCustomerId(int customerId, MySqlConnection connection)
        {
            Response response = new Response();
            MySqlCommand getCartItemCmd = new MySqlCommand(
                "SELECT cart.cartID, cart.id_customer, cart.idsp, cart.quantity, cart.dateAdded, sanpham.anhsp, sanpham.tensp, sanpham.giaban " +
                "FROM cart " +
                "INNER JOIN sanpham ON cart.idsp = sanpham.idsp " +
                "WHERE cart.id_customer = @customerId", connection);
            getCartItemCmd.Parameters.AddWithValue("@customerId", customerId);
            MySqlDataAdapter adapter = new MySqlDataAdapter(getCartItemCmd);

            DataTable dataTable = new DataTable();
            connection.Open();
            adapter.Fill(dataTable);
            List<Cart> arrayCart = new List<Cart>();
            if (dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Cart cart = new Cart();
                    cart.cartID = Convert.ToInt32(dataTable.Rows[i]["cartID"]);
                    cart.id_customer = Convert.ToInt32(dataTable.Rows[i]["id_customer"]);
                    cart.idsp = Convert.ToInt32(dataTable.Rows[i]["idsp"]);
                    cart.tensp = Convert.ToString(dataTable.Rows[i]["tensp"]);
                    cart.anhsp = Convert.ToString(dataTable.Rows[i]["anhsp"]);
                    cart.giaban = Convert.ToDecimal(dataTable.Rows[i]["giaban"]);
                    cart.quantity = Convert.ToInt32(dataTable.Rows[i]["quantity"]);
                    arrayCart.Add(cart);
                }
            }
            if (arrayCart.Count > 0)
            {
                response.StatusCode = 200;
                response.StatusMessage = "Tất cả sản phẩm có trong giỏ hàng";
                response.arrayCart = arrayCart;
                return response;
            }
            else
            {
                return null;
            }
            connection.Close();
            return response;
        }

    }
}
