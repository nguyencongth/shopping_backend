using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
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
            MySqlConnection connection = new MySqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectOrder.CreateOrder(connection, order);
            return response;
        }

        [HttpGet]
        [Route("getOrder")]
        public Response getOrder(int customerID)
        {
            Response response = new Response();
            ConnectOrder connectOrder = new ConnectOrder();
            MySqlConnection connection = new MySqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectOrder.getOrderByIdCustomer(connection, customerID);
            return response;
        }
        [HttpDelete]
        [Route("deleteOrder")]
        public Response deleteOrder(int orderID)
        {
            Response response = new Response();
            ConnectOrder connectOrder = new ConnectOrder();
            MySqlConnection connection = new MySqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectOrder.deleteOrder(connection, orderID);
            return response;
        }
    }
}
