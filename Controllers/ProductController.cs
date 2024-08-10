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
        public Response getProductNew(int priceRange, int page, int pageSize)
        {
            Response response = new Response();
            ConnectProduct connectProduct = new ConnectProduct();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectProduct.getProductNew(connection, priceRange, page, pageSize);
            return response;
        }
        
        [HttpGet]
        [Route("productNewHome")]
        public Response getProductNew()
        {
            Response response = new Response();
            ConnectProduct connectProduct = new ConnectProduct();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectProduct.getProductNewHome(connection);
            return response;
        }
        
        [HttpGet]
        [Route("productSaleHome")]
        public Response getProductSaleHome()
        {
            Response response = new Response();
            ConnectProduct connectProduct = new ConnectProduct();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectProduct.getProductSaleHome(connection);
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

        [HttpGet]
        [Route("getProductAdmin")]
        public Response getProductAdmin()
        {
            Response response = new Response();
            ConnectProduct connectProduct = new ConnectProduct();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectProduct.getProductAdmin(connection);
            return response;
        }

        [HttpDelete]
        [Route("deleteProduct")]
        public Response deleteProduct(int productId)
        {
            Response response = new Response();
            ConnectProduct connectProduct = new ConnectProduct();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectProduct.deleteProduct(connection, productId);
            return response;
        }
        
        [HttpPatch]
        [Route("updateInfoProduct")]
        public Response updateInfoProduct(Product products)
        {
            Response response = new Response();
            ConnectProduct connectProduct = new ConnectProduct();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectProduct.updateInfoProduct(products, connection);
            return response;
        }
        
        [HttpPost]
        [Route("addNewProduct")]
        public Response addNewProduct(Product products)
        {
            Response response = new Response();
            ConnectProduct connectProduct = new ConnectProduct();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectProduct.addNewProduct(products, connection);
            return response;
        }
        
        [HttpGet]
        [Route("top5ProductsBestSelling")]
        public Response top5ProductsBestSelling()
        {
            Response response = new Response();
            ConnectProduct connectProduct = new ConnectProduct();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectProduct.top5ProductsBestSelling(connection);
            return response;
        }
        
        [HttpGet]
        [Route("totalNumberOfProductSold")]
        public Response totalNumberOfProductSold()
        {
            Response response = new Response();
            ConnectProduct connectProduct = new ConnectProduct();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectProduct.totalNumberOfProductsSoleInMonth(connection);
            return response;
        }
        
        [HttpGet]
        [Route("totalRevenueInMonth")]
        public Response totalRevenueInMonth(int year)
        {
            Response response = new Response();
            ConnectProduct connectProduct = new ConnectProduct();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectProduct.revenue(year,connection);
            return response;
        }
    }
}
