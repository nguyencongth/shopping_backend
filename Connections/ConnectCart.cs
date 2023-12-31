﻿using Microsoft.Data.SqlClient;
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
            connection.Open();
            MySqlCommand checkCartCmd = new MySqlCommand("SELECT COUNT(*) FROM cart WHERE id_customer = @customerID AND idsp = @productID", connection);
            checkCartCmd.Parameters.AddWithValue("@customerID", cart.id_customer);
            checkCartCmd.Parameters.AddWithValue("@productID", cart.idsp);

            int cartCount = Convert.ToInt32(checkCartCmd.ExecuteScalar());

            if(cartCount > 0 )
            {
                MySqlCommand updateCmd = new MySqlCommand("UPDATE cart SET quantity = quantity + @newQuantity WHERE id_customer = @customerID and idsp = @productID", connection);
                updateCmd.Parameters.AddWithValue("@newQuantity", cart.quantity);
                updateCmd.Parameters.AddWithValue("@customerID", cart.id_customer);
                updateCmd.Parameters.AddWithValue("@productID", cart.idsp);
                int rowsUPdated = updateCmd.ExecuteNonQuery();

                if(rowsUPdated > 0 )
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
                MySqlCommand command = new MySqlCommand("sp_add_cart", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("IN_id_customer", cart.id_customer);
                command.Parameters.AddWithValue("IN_idsp", cart.idsp);
                command.Parameters.AddWithValue("IN_quantity", cart.quantity);
                command.Parameters.AddWithValue("IN_dateAdded", DateTime.Now);

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
        public Response UpdateCartQuantity(MySqlConnection connection,int id_customer, int idsp, int newQuantity)
        {
            Response response = new Response();
            MySqlCommand checkCartCmd = new MySqlCommand("SELECT COUNT(*) FROM cart WHERE id_customer = @customerID AND idsp = @productID", connection);
            checkCartCmd.Parameters.AddWithValue("@customerID", id_customer);
            checkCartCmd.Parameters.AddWithValue("@productID", idsp);

            connection.Open();

            int cartCount = Convert.ToInt32(checkCartCmd.ExecuteScalar());

            if (cartCount > 0)
            {
                MySqlCommand updateCmd = new MySqlCommand("UPDATE Cart SET quantity = @newQuantity WHERE id_customer = @customerID AND idsp = @productID", connection);
                updateCmd.Parameters.AddWithValue("@newQuantity", newQuantity);
                updateCmd.Parameters.AddWithValue("@customerID", id_customer);
                updateCmd.Parameters.AddWithValue("@productID", idsp);
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
        public Response deleteCart(MySqlConnection connection, int customerID)
        {
            Response response = new Response();
            MySqlCommand removeCmd = new MySqlCommand("DELETE FROM cart WHERE id_customer = @customerId", connection);
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

        public Response deleteCartItem(MySqlConnection connection, int customerID, int productID)
        {
            Response response = new Response();
            connection.Open();
            MySqlCommand removeCmd = new MySqlCommand("DELETE FROM cart WHERE id_customer = @customerId AND idsp = @productId", connection);
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

        public Response GetCartItemsByCustomerId(int customerId, MySqlConnection connection)
        {
            Response response = new Response();
            connection.Open();
            MySqlCommand getCartItemCmd = new MySqlCommand(
                "SELECT cart.cartID, cart.id_customer, cart.idsp, cart.quantity, cart.dateAdded, sanpham.anhsp, sanpham.tensp, sanpham.giaban " +
                "FROM cart " +
                "INNER JOIN sanpham ON cart.idsp = sanpham.idsp " +
                "WHERE cart.id_customer = @customerId", connection);
            getCartItemCmd.Parameters.AddWithValue("@customerId", customerId);
            MySqlDataAdapter adapter = new MySqlDataAdapter(getCartItemCmd);

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
                response.arrayCart = cartEmpty;
                return response;
            }
        }

    }
}
