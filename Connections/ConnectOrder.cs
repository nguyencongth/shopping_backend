using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.X509;
using System.Data;
using System.Transactions;
using WebServiceShopping.Models;

namespace WebServiceShopping.Connections
{
    public class ConnectOrder
    {
        public Response CreateOrder(MySqlConnection connection, Orders orders)
        {
            Response response = new Response();

            connection.Open();
            using(MySqlTransaction transaction = connection.BeginTransaction())
            {
                try
                {
                    MySqlCommand OrderCmd = new MySqlCommand("sp_create_order", connection, transaction);
                    OrderCmd.CommandType = CommandType.StoredProcedure;
                    OrderCmd.Parameters.AddWithValue("IN_CustomerId", orders.id_customer);
                    OrderCmd.Parameters.AddWithValue("IN_OrderDate", DateTime.Now);
                    OrderCmd.Parameters.AddWithValue("IN_shippingAddress", orders.shippingAddress);
                    OrderCmd.Parameters.AddWithValue("IN_TotalAmount", 0);
                    OrderCmd.Parameters.AddWithValue("IN_paymentMethod", orders.paymentMethod);
                    OrderCmd.Parameters.AddWithValue("IN_orderStatus", 0);
                    OrderCmd.ExecuteNonQuery();

                    MySqlCommand getOrderIDCmd = new MySqlCommand("SELECT LAST_INSERT_ID()", connection, transaction);
                    int orderID = Convert.ToInt32(getOrderIDCmd.ExecuteScalar());

                    List<OrderItem> orderItems = new List<OrderItem>();

                    foreach (var orderItem in orders.orderItems)
                    {
                        MySqlCommand orderItemCmd = new MySqlCommand("sp_create_order_item", connection, transaction);
                        orderItemCmd.CommandType = CommandType.StoredProcedure;
                        orderItemCmd.Parameters.AddWithValue("IN_order_id", orderID);
                        orderItemCmd.Parameters.AddWithValue("IN_product_id", orderItem.idsp);
                        orderItemCmd.Parameters.AddWithValue("IN_quantity", orderItem.quantity);

                        Product product = GetProductInfo(connection, orderItem.idsp);
                        if (product != null)
                        {
                            decimal subtotal = product.giaban * orderItem.quantity;
                            orderItemCmd.Parameters.AddWithValue("IN_subtotal", subtotal);
                            orderItem.subtotal = subtotal;
                        }
                        orderItemCmd.ExecuteNonQuery();
                        orderItems.Add(orderItem);
                    }

                    MySqlCommand updateTotalAmountCmd = new MySqlCommand("sp_update_total_amount", connection, transaction);
                    updateTotalAmountCmd.CommandType = CommandType.StoredProcedure;
                    updateTotalAmountCmd.Parameters.AddWithValue("IN_order_id", orderID);
                    updateTotalAmountCmd.ExecuteNonQuery();

                    transaction.Commit();
                   
                    response.StatusCode = 200;
                    response.StatusMessage = "Thêm đơn hàng thành công";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    response.StatusCode = 100;
                    response.StatusMessage = "Lỗi khi thêm đơn hàng"+ ex.Message;
                }
            }
            return response;

        }

        public Response deleteOrder(MySqlConnection connection, int orderID)
        {
            Response response = new Response();

            try
            {
                connection.Open();

                MySqlTransaction transaction = connection.BeginTransaction();

                MySqlCommand deleteOrderItems = new MySqlCommand("DELETE FROM order_item WHERE order_id = @orderID", connection, transaction);
                deleteOrderItems.Parameters.AddWithValue("@orderID", orderID);
                deleteOrderItems.ExecuteNonQuery();

                MySqlCommand deleteOrder = new MySqlCommand("DELETE FROM orders WHERE order_id = @orderID", connection, transaction);
                deleteOrder.Parameters.AddWithValue("@orderID", orderID);
                deleteOrder.ExecuteNonQuery();

                transaction.Commit();

                response.StatusCode = 200;
                response.StatusMessage = "Xóa đơn hàng thành công.";
            }
            catch (Exception ex)
            {
                response.StatusCode = 400;
                response.StatusMessage = "Lỗi khi xóa đơn hàng. " + ex.Message;
            }
            finally
            {
                connection.Close();
            }

            return response;
        }

        public Response getOrderByIdCustomer(MySqlConnection connection, int customerID)
        {
            Response response = new Response();
            connection.Open();
            List<Orders> orders = new List<Orders>();
            MySqlCommand getOrderCmd = new MySqlCommand("getOrderByCustomerID", connection);
            getOrderCmd.CommandType = CommandType.StoredProcedure;
            getOrderCmd.Parameters.AddWithValue("IN_CustomerID", customerID);
            using (MySqlDataReader reader = getOrderCmd.ExecuteReader())
            {
                Orders currentOrder = null;

                while (reader.Read())
                {
                    int orderID = reader.GetInt32("order_id");

                    if (currentOrder == null || currentOrder.order_id != orderID)
                    {
                        currentOrder = new Orders
                        {
                            order_id = orderID,
                            id_customer = reader.GetInt32("id_customer"),
                            order_date = reader.GetDateTime("order_date"),
                            shippingAddress = reader.GetString("shippingAddress"),
                            total_amount = reader.GetDecimal("total_amount"),
                            paymentMethod = reader.GetString("paymentMethod"),
                            orderStatus = reader.GetInt32("orderStatus"),
                            orderItems = new List<OrderItem>()
                        };

                        orders.Add(currentOrder);
                    }

                    currentOrder.orderItems.Add(new OrderItem
                    {
                        order_item_id = reader.GetInt32("order_item_id"),
                        idsp = reader.GetInt32("idsp"),
                        quantity = reader.GetInt32("quantity"),
                        subtotal = reader.GetDecimal("subtotal")
                    });
                }
            }
            if (orders.Count > 0)
            {
                response.StatusCode = 200;
                response.StatusMessage = "Danh sách đơn hàng";
                response.arrayOrders = orders;
            }
            else
            {
                response.StatusCode = 404;
                response.StatusMessage = "Không tìm thấy đơn hàng";
            }
            connection.Close();
            return response;
        }
        public Response updateOrderStatus(MySqlConnection connection,int orderId, int newOrderStatus)
        {
            Response response = new Response();

            try
            {
                connection.Open();

                MySqlCommand updateOrderStatus = new MySqlCommand("UPDATE orders SET orderStatus = @newOrderStatus WHERE order_id = @orderId", connection);
                updateOrderStatus.Parameters.AddWithValue("@orderId", orderId);
                updateOrderStatus.Parameters.AddWithValue("@newOrderStatus", newOrderStatus);

                int rowUpdate = updateOrderStatus.ExecuteNonQuery();
                if(rowUpdate > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "Cập nhật trạng thái đơn hàng thành công.";
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 400;
                response.StatusMessage = "Lỗi khi cập nhật trạng thái đơn hàng. " + ex.Message;
            }
            finally
            {
                connection.Close();
            }
            return response;
        }
        Product GetProductInfo(MySqlConnection connection, int productId)
        {
            Product product = null;
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM sanpham WHERE idsp = @ProductId", connection);
            cmd.Parameters.AddWithValue("@ProductId", productId);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    product = new Product
                    {
                        idsp = reader.GetInt32("idsp"),
                        giaban = reader.GetDecimal("giaban"),
                    };
                }
            }
            return product;
        }
    }
}
