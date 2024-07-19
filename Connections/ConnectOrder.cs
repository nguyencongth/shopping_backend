using System.Data;
using Microsoft.Data.SqlClient;
using WebServiceShopping.Models;

namespace WebServiceShopping.Connections
{
    public class ConnectOrder
    {
        public Response CreateOrder(SqlConnection connection, Orders orders)
        {
            Response response = new Response();
            connection.Open();
                try
                {
                    SqlCommand OrderCmd = new SqlCommand("sp_create_order", connection);
                    OrderCmd.CommandType = CommandType.StoredProcedure;
                    OrderCmd.Parameters.AddWithValue("@IN_CustomerId", orders.customerId);
                    //OrderCmd.Parameters.AddWithValue("@IN_OrderDate", DateTime.Now);
                    OrderCmd.Parameters.AddWithValue("@IN_TotalAmount", 0);
                    OrderCmd.Parameters.AddWithValue("@IN_paymentMethod", orders.paymentMethod);
                    //OrderCmd.Parameters.AddWithValue("@IN_orderStatus", 0);
                    SqlParameter outputParam = new SqlParameter("@OrderId", SqlDbType.Int);
                    outputParam.Direction = ParameterDirection.Output;
                    OrderCmd.Parameters.Add(outputParam);
                    OrderCmd.ExecuteNonQuery();

                    int orderID = Convert.ToInt32(OrderCmd.Parameters["@OrderId"].Value);

                    List<OrderItem> orderItems = new List<OrderItem>();
                    foreach (var orderItem in orders.orderItems)
                    {
                        SqlCommand orderItemCmd = new SqlCommand("sp_create_order_item", connection);
                        orderItemCmd.CommandType = CommandType.StoredProcedure;
                        orderItemCmd.Parameters.AddWithValue("@IN_order_id", orderID);
                        orderItemCmd.Parameters.AddWithValue("@IN_product_id", orderItem.productId);
                        orderItemCmd.Parameters.AddWithValue("@IN_quantity", orderItem.quantity);

                        Product product = GetProductInfo(connection, orderItem.productId);
                        if (product != null)
                        {
                            decimal subtotal = product.price * orderItem.quantity;
                            orderItemCmd.Parameters.AddWithValue("@IN_subtotal", subtotal);
                            orderItem.subtotal = subtotal;
                        }

                        orderItemCmd.ExecuteNonQuery();
                        orderItems.Add(orderItem);
                    }

                    SqlCommand updateTotalAmountCmd = new SqlCommand("sp_update_total_amount", connection);
                    updateTotalAmountCmd.CommandType = CommandType.StoredProcedure;
                    updateTotalAmountCmd.Parameters.AddWithValue("@order_id", orderID);
                    updateTotalAmountCmd.ExecuteNonQuery();
                    
                    response.StatusCode = 200;
                    response.StatusMessage = "Thêm đơn hàng thành công";
                }
                catch (Exception ex)
                {
                    response.StatusCode = 100;
                    response.StatusMessage = "Lỗi khi thêm đơn hàng" + ex.Message;
                }
            return response;
        }

        public Response deleteOrder(SqlConnection connection, int orderID)
        {
            Response response = new Response();
            try
            {
                connection.Open();

                SqlTransaction transaction = connection.BeginTransaction();

                SqlCommand deleteOrderItems = new SqlCommand("DELETE FROM orderItem WHERE orderId = @orderID", connection, transaction);
                deleteOrderItems.Parameters.AddWithValue("@orderID", orderID);
                deleteOrderItems.ExecuteNonQuery();

                SqlCommand deleteOrder = new SqlCommand("DELETE FROM orders WHERE orderId = @orderID", connection, transaction);
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

        public Response getOrderByIdCustomer(SqlConnection connection, int customerID)
        {
            Response response = new Response();
            connection.Open();
            List<Orders> orders = new List<Orders>();
            SqlCommand getOrderCmd = new SqlCommand("getOrderByCustomerID", connection);
            getOrderCmd.CommandType = CommandType.StoredProcedure;
            getOrderCmd.Parameters.AddWithValue("IN_CustomerID", customerID);
            using (SqlDataReader reader = getOrderCmd.ExecuteReader())
            {
                Orders currentOrder = null;

                while (reader.Read())
                {
                    int orderID = reader.GetInt32("orderId");

                    if (currentOrder == null || currentOrder.orderId != orderID)
                    {
                        currentOrder = new Orders
                        {
                            orderId = orderID,
                            customerId = reader.GetInt32("customerId"),
                            orderDate = reader.GetDateTime("orderDate"),
                            totalAmount = reader.GetDecimal("totalAmount"),
                            paymentMethod = reader.GetString("paymentMethod"),
                            orderStatus = reader.GetInt32("orderStatus"),
                            orderItems = new List<OrderItem>()
                        };

                        orders.Add(currentOrder);
                    }

                    currentOrder.orderItems.Add(new OrderItem
                    {
                        orderItemId = reader.GetInt32("orderItemId"),
                        productId = reader.GetInt32("productId"),
                        quantity = reader.GetInt32("quantity"),
                        subtotal = reader.GetDecimal("subtotal"),
                        productName = reader.GetString("productName"),
                        imageProduct = reader.GetString("imageProduct")
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
        public Response updateOrderStatus(SqlConnection connection,int orderId, int newOrderStatus)
        {
            Response response = new Response();

            try
            {
                connection.Open();

                SqlCommand updateOrderStatus = new SqlCommand("UPDATE orders SET orderStatus = @newOrderStatus WHERE orderId = @orderId", connection);
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
        Product GetProductInfo(SqlConnection connection, int productId)
        {
            Product product = null;
            using(SqlTransaction transaction = connection.BeginTransaction())
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM products WHERE productId = @ProductId", connection, transaction);
                cmd.Parameters.AddWithValue("@ProductId", productId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        product = new Product
                        {
                            productId = reader.GetInt32("productId"),
                            price = reader.GetDecimal("price"),
                        };
                    }
                }
                transaction.Commit();
                return product;
            }
        }

        public Response getAllOrder(SqlConnection connection)
        {
            Response response = new Response();
            connection.Open();
            List<Orders> orders = new List<Orders>();
            SqlCommand getOrderCmd = new SqlCommand("sp_get_all_order", connection);
            getOrderCmd.CommandType = CommandType.StoredProcedure;
            using (SqlDataReader reader = getOrderCmd.ExecuteReader())
            {
                Orders currentOrder = null;

                while (reader.Read())
                {
                    int orderID = reader.GetInt32("orderId");

                    if (currentOrder == null || currentOrder.orderId != orderID)
                    {
                        currentOrder = new Orders
                        {
                            orderId = orderID,
                            customerId = reader.GetInt32("customerId"),
                            orderDate = reader.GetDateTime("orderDate"),
                            totalAmount = reader.GetDecimal("totalAmount"),
                            paymentMethod = reader.GetString("paymentMethod"),
                            orderStatus = reader.GetInt32("orderStatus"),
                            orderItems = new List<OrderItem>()
                        };

                        orders.Add(currentOrder);
                    }

                    currentOrder.orderItems.Add(new OrderItem
                    {
                        orderItemId = reader.GetInt32("orderItemId"),
                        productId = reader.GetInt32("productId"),
                        quantity = reader.GetInt32("quantity"),
                        subtotal = reader.GetDecimal("subtotal"),
                        productName = reader.GetString("productName"),
                        price = reader.GetDecimal("price"),
                        imageProduct = reader.GetString("imageProduct")
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
                response.StatusCode = 400;
                response.StatusMessage = "Không tìm thấy đơn hàng";
            }
            connection.Close();
            return response;

        }
    }
}
