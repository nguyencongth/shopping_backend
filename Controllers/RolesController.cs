using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WebServiceShopping.Connections;
using WebServiceShopping.Models;

namespace WebServiceShopping.Controllers;

public class RolesController : Controller
{
    private readonly IConfiguration _configuration; // Khởi tạo cấu hình và tên cấu hình

    public RolesController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    [HttpGet]
    [Route("getRoles")]
    public Response getRoles()
    {
        Response response = new Response();
        ConnectRoles connectRoles = new ConnectRoles();
        SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice"));
        response = connectRoles.getRoles(connection);
        return response;
    }
}