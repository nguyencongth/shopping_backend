using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WebServiceShopping.Connections;
using WebServiceShopping.Models;

namespace WebServiceShopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public OrdersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("order")]
        public Response Order(Orders order)
        {
            Response response = new Response();
            ConnectOrder connectOrder = new ConnectOrder();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectOrder.CreateOrder(connection, order);
            return response;
        }

        [HttpGet]
        [Route("getOrder")]
        public Response getOrder(int customerID)
        {
            Response response = new Response();
            ConnectOrder connectOrder = new ConnectOrder();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectOrder.getOrderByIdCustomer(connection, customerID);
            return response;
        }
        [HttpDelete]
        [Route("deleteOrder")]
        public Response deleteOrder(int orderID)
        {
            Response response = new Response();
            ConnectOrder connectOrder = new ConnectOrder();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectOrder.deleteOrder(connection, orderID);
            return response;
        }

        [HttpPatch]
        [Route("updateOrderStatus")]
        public Response updateOrderStatus(int orderId, int newOrderStatus)
        {
            Response response = new Response();
            ConnectOrder connectOrder = new ConnectOrder();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectOrder.updateOrderStatus(connection, orderId, newOrderStatus);
            return response;
        }
    }
}
