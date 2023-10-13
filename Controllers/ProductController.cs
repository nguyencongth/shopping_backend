using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using WebServiceShopping.Connections;
using WebServiceShopping.Models;

namespace WebServiceShopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ProductController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("getProductId")]
        public Response getProductId(int idsp)
        {
            Response response = new Response();
            ConnectProduct connectProduct = new ConnectProduct();
            MySqlConnection mySqlConnection = new MySqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectProduct.getProductId(mySqlConnection, idsp);
            return response;
        }

        [HttpGet]
        [Route("all")]
        public Response all(int page, int pageSize)
        {
            Response response = new Response();
            ConnectProduct connectProduct = new ConnectProduct();
            MySqlConnection connection = new MySqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectProduct.productAll(connection, page, pageSize);
            return response;
        }

        [HttpGet]
        [Route("productDress")]
        public Response productDress(int page, int pageSize)
        {
            Response response = new Response();
            ConnectProduct connectProduct = new ConnectProduct();
            MySqlConnection connection = new MySqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectProduct.product_dress(connection, page, pageSize);
            return response;
        }

        [HttpGet]
        [Route("productShirt")]
        public Response productShirt(int page, int pageSize)
        {
            Response response = new Response();
            ConnectProduct connectProduct = new ConnectProduct();
            MySqlConnection connection = new MySqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectProduct.product_shirt(connection, page, pageSize);
            return response;
        }
    }
}
