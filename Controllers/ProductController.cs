using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
        public Response getProductId(int productID)
        {
            Response response = new Response();
            ConnectProduct connectProduct = new ConnectProduct();
            SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectProduct.getProductId(sqlConnection, productID);
            return response;
        }

        [HttpGet]
        [Route("getProductByCategoryId")]
        public Response getProductByCategoryId(int categoryId, int priceRange, int page, int pageSize)
        {
            Response response = new Response();
            ConnectProduct connectProduct = new ConnectProduct();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectProduct.productByCategoryId(connection,categoryId, priceRange, page, pageSize);
            return response;
        }

        [HttpGet]
        [Route("all")]
        public Response all(int priceRange, int page, int pageSize)
        {
            Response response = new Response();
            ConnectProduct connectProduct = new ConnectProduct();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectProduct.productAll(connection, priceRange, page, pageSize);
            return response;
        }

        [HttpGet]
        [Route("productNew")]
        public Response getProductNew()
        {
            Response response = new Response();
            ConnectProduct connectProduct = new ConnectProduct();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectProduct.getProductNew(connection);
            return response;
        }

        [HttpGet]
        [Route("productDress")]
        public Response productDress(int page, int pageSize)
        {
            Response response = new Response();
            ConnectProduct connectProduct = new ConnectProduct();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectProduct.product_dress(connection, page, pageSize);
            return response;
        }

        [HttpGet]
        [Route("productShirt")]
        public Response productShirt(int page, int pageSize)
        {
            Response response = new Response();
            ConnectProduct connectProduct = new ConnectProduct();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectProduct.product_shirt(connection, page, pageSize);
            return response;
        }

        [HttpGet]
        [Route("searchProduct")]
        public Response searchProduct(string keyword, int page, int pageSize)
        {
            Response response = new Response();
            ConnectProduct connectProduct = new ConnectProduct();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectProduct.SearchProduct(connection, keyword, page, pageSize);
            return response;
        }
        [HttpGet]
        [Route("filterProductsByPrice")]
        public Response filterProductsByPrice(int priceRange, int page, int pageSize)
        {
            Response response = new Response();
            ConnectProduct connectProduct = new ConnectProduct();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectProduct.FilterProductsByPrice(connection, priceRange, page, pageSize);
            return response;
        }
    }
}
