using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using WebServiceShopping.Connections;
using WebServiceShopping.Models;

namespace WebServiceShopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public CartController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("addToCart")]
        public Response Cart(Cart cart)
        {
            Response response = new Response();
            ConnectCart connectCart = new ConnectCart();
            MySqlConnection connection = new MySqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectCart.AddToCart(cart, connection);
            return response;
        }

        [HttpPatch]
        [Route("updateCartQuanrtity")]
        public Response UpdateCartQuantity(Cart cart)
        {
            Response response = new Response();
            ConnectCart connectCart = new ConnectCart();
            MySqlConnection connection = new MySqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectCart.UpdateCartQuantity(cart, connection);
            return response;
        }
        [HttpDelete]
        [Route("deleteCartItem")]
        public Response DeleteCartItem(int customerID, int productID)
        {
            Response response = new Response();
            ConnectCart connectCart = new ConnectCart();
            MySqlConnection connection = new MySqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectCart.deleteCartItem(connection, customerID, productID);
            return response;
        }

        [HttpGet]
        [Route("showCartItem")]
        public Response ShowCartItem(int customerID)
        {
            Response response = new Response();
            ConnectCart connectCart = new ConnectCart();
            MySqlConnection connection = new MySqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectCart.GetCartItemsByCustomerId(customerID, connection);
            return response;
        }
    }
}
