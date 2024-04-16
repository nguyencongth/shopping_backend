using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectCart.AddToCart(cart, connection);
            return response;
        }

        [HttpPatch]
        [Route("updateCartQuantity")]
        public Response UpdateCartQuantity(int customerID, int productID, int newQuantity)
        {
            Response response = new Response();
            ConnectCart connectCart = new ConnectCart();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectCart.UpdateCartQuantity(connection, customerID, productID, newQuantity);
            return response;
        }

        [HttpDelete]
        [Route("deleteCart")]
        public Response DeleteCart(int customerID)
        {
            Response response = new Response();
            ConnectCart connectCart = new ConnectCart();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectCart.deleteCart(connection, customerID);
            return response;
        }

        [HttpDelete]
        [Route("deleteCartItem")]
        public Response DeleteCartItem(int customerID, int productID)
        {
            Response response = new Response();
            ConnectCart connectCart = new ConnectCart();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectCart.deleteCartItem(connection, customerID, productID);
            return response;
        }

        [HttpGet]
        [Route("showCartItem")]
        public Response ShowCartItem(int customerID)
        {
            Response response = new Response();
            ConnectCart connectCart = new ConnectCart();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice"));
            response = connectCart.GetCartItemsByCustomerId(customerID, connection);
            return response;
        }
    }
}
