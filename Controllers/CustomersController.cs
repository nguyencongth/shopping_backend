using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using WebServiceShopping.Connections;
using WebServiceShopping.Models;

namespace WebServiceShopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public CustomersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpPost]
        [Route("register")]
        public Response register(Customers customer)
        {
            Response response = new Response();
            ConnectCustomer connectCustomer = new ConnectCustomer();
            MySqlConnection connection = new MySqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectCustomer.register(customer, connection);
            return response;

        }
        [HttpPost]
        [Route("Login")]
        public Response login(Login login)
        {
            Response response = new Response();
            ConnectCustomer connectCustomer = new ConnectCustomer();
            MySqlConnection connection = new MySqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectCustomer.Login(login, connection);
            return response;
        }
        [HttpGet]
        [Route("customerAll")]
        public Response customerAll()
        {
            Response response = new Response();
            ConnectCustomer connectCustomer = new ConnectCustomer();
            MySqlConnection connection = new MySqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectCustomer.customerAll(connection);
            return response;
        }

        [HttpPatch]
        [Route("updateInfo")]
        public Response updateInfo(Customers customer)
        {
            Response response = new Response();
            ConnectCustomer connectCustomer = new ConnectCustomer();
            MySqlConnection connection = new MySqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectCustomer.updateInfo(customer, connection);
            return response;
        }

        [HttpGet]
        [Route("getCustomerById")]
        public Response getCustomerById(int CustomerID)
        {
            Response response = new Response();
            ConnectCustomer connectCustomer = new ConnectCustomer();
            MySqlConnection connection = new MySqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectCustomer.getCustomerById(connection, CustomerID);
            return response;
        }
    }
}
