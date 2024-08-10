using Microsoft.Data.SqlClient;
using System.Data;
using WebServiceShopping.Models;

namespace WebServiceShopping.Connections
{
    public class ConnectCart
    {
        public Response AddToCart(Cart cart, SqlConnection connection)
        {
            Response response = new Response();
            connection.Open();
            SqlCommand checkCartCmd = new SqlCommand("SELECT COUNT(*) FROM cart WHERE customerId = @customerID AND productId = @productID", connection);
            checkCartCmd.Parameters.AddWithValue("@customerID", cart.customerId);
            checkCartCmd.Parameters.AddWithValue("@productID", cart.productId);
            int cartCount = (int)checkCartCmd.ExecuteScalar();

            if(cartCount > 0 )
            {
                SqlCommand updateCmd = new SqlCommand("UPDATE cart SET quantity = quantity + @newQuantity WHERE customerId = @customerID AND productId = @productID", connection);
                updateCmd.Parameters.AddWithValue("@newQuantity", cart.quantity);
                updateCmd.Parameters.AddWithValue("@customerID", cart.customerId);
                updateCmd.Parameters.AddWithValue("@productID", cart.productId);
                int rowsUpdated = updateCmd.ExecuteNonQuery();

                if(rowsUpdated > 0 )
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
                SqlCommand command = new SqlCommand("sp_add_cart", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@customerID", cart.customerId);
                command.Parameters.AddWithValue("@productID", cart.productId);
                command.Parameters.AddWithValue("@quantity", cart.quantity);
                //command.Parameters.AddWithValue("@IN_dateAdded", DateTime.Now);

                int i = command.ExecuteNonQuery();

                if (i > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "Thêm sản phẩm vào giỏ hàng thành công";
                }
                else
                {
                    response.StatusCode = 400;
                    response.StatusMessage = "Lỗi khi thêm sản phẩm vào giỏ hàng";
                }
            }
            connection.Close();
            return response;
        }
        public Response UpdateCartQuantity(SqlConnection connection,int customerID, int productID, int newQuantity)
        {
            Response response = new Response();
            SqlCommand checkCartCmd = new SqlCommand("SELECT COUNT(*) FROM cart WHERE customerId = @customerID AND productId = @productID", connection);
            checkCartCmd.Parameters.AddWithValue("@customerID", customerID);
            checkCartCmd.Parameters.AddWithValue("@productID", productID);

            connection.Open();

            int cartCount = Convert.ToInt32(checkCartCmd.ExecuteScalar());

            if (cartCount > 0)
            {
                SqlCommand updateCmd = new SqlCommand("UPDATE cart SET quantity = @newQuantity WHERE customerId = @customerID AND productId = @productID", connection);
                updateCmd.Parameters.AddWithValue("@newQuantity", newQuantity);
                updateCmd.Parameters.AddWithValue("@customerID", customerID);
                updateCmd.Parameters.AddWithValue("@productID", productID);
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
        public Response deleteCart(SqlConnection connection, int customerID)
        {
            Response response = new Response();
            SqlCommand removeCmd = new SqlCommand("DELETE FROM cart WHERE customerId = @customerId", connection);
            removeCmd.Parameters.AddWithValue("@customerId", customerID);

            connection.Open();

            int rowsDeleted = removeCmd.ExecuteNonQuery();
            if (rowsDeleted > 0)
            {
                response.StatusCode = 200;
                response.StatusMessage = "Xóa tất cả sản phẩm khỏi giỏ hàng thành công.";
                return response;
            }
            else
            {
                response.StatusCode = 400;
                response.StatusMessage = "Lỗi khi xóa tất sản phẩm khỏi giỏ hàng.";
            }
            connection.Close();
            return response;
        }

        public Response deleteCartItem(SqlConnection connection, int customerID, int productID)
        {
            Response response = new Response();
            connection.Open();
            SqlCommand removeCmd = new SqlCommand("DELETE FROM cart WHERE customerId = @customerId AND productId = @productId", connection);
            removeCmd.Parameters.AddWithValue("@customerId", customerID);
            removeCmd.Parameters.AddWithValue("@productID", productID);

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

        public Response GetCartItemsByCustomerId(int customerId, SqlConnection connection)
        {
            Response response = new Response();
            connection.Open();
            SqlCommand getCartItemCmd = new SqlCommand(
                "SELECT cart.cartId, cart.customerId, cart.productId, cart.quantity, cart.dateAdded, products.imageProduct, products.productName, products.price, products.discountPercentage, price - (price * discountPercentage / 100) AS discountedPrice " +
                "FROM cart " +
                "INNER JOIN products ON cart.productId = products.productId " +
                "WHERE cart.customerId = @customerId", connection);
            getCartItemCmd.Parameters.AddWithValue("@customerId", customerId);
            SqlDataAdapter adapter = new SqlDataAdapter(getCartItemCmd);

            DataTable dataTable = new DataTable();
            
            adapter.Fill(dataTable);
            connection.Close();
            List<Cart> arrayCart = new List<Cart>();
            List<Cart> cartEmpty = new List<Cart>();

            if (dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Cart cart = new Cart();
                    cart.cartId = Convert.ToInt32(dataTable.Rows[i]["cartId"]);
                    cart.customerId = Convert.ToInt32(dataTable.Rows[i]["customerId"]);
                    cart.productId = Convert.ToInt32(dataTable.Rows[i]["productId"]);
                    cart.productName = Convert.ToString(dataTable.Rows[i]["productName"]);
                    cart.imageProduct = Convert.ToString(dataTable.Rows[i]["imageProduct"]);
                    cart.price = Convert.ToDecimal(dataTable.Rows[i]["price"]);
                    cart.quantity = Convert.ToInt32(dataTable.Rows[i]["quantity"]);
                    cart.discountPercentage = Convert.ToDecimal(dataTable.Rows[i]["discountPercentage"]);
                    cart.discountedPrice = Convert.ToDecimal(dataTable.Rows[i]["discountedPrice"]);
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
                response.arrayCart = cartEmpty;
                return response;
            }
        }

    }
}
