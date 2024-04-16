using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WebServiceShopping.Connections;
using WebServiceShopping.Models;

namespace WebServiceShopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductTypeController : ControllerBase
    {
        private readonly IConfiguration _configuration; // Khởi tạo cấu hình và tên cấu hình

        public ProductTypeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("getProductType")]
        public Response getProductType()
        {
            Response response = new Response();
            ConnectProductType connectProductType = new ConnectProductType();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice").ToString());
            response = connectProductType.productTpyeAll(connection);
            return response;
        }

        [HttpGet]
        [Route("getCategoryById")]
        public Response getCategoryById(int categoryId)
        {
            Response response = new Response();
            ConnectProductType connectProductType = new ConnectProductType();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice").ToString());
            response = connectProductType.getCategoryById(connection, categoryId);
            return response;
        }
    }
}
