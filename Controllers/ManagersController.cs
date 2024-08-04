using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WebServiceShopping.Connections;
using WebServiceShopping.Models;

namespace WebServiceShopping.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ManagersController : Controller
{
    private readonly IConfiguration _configuration; // Khởi tạo cấu hình và tên cấu hình

    public ManagersController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    [HttpPost]
    [Route("register")]
    public Response register(Managers manager)
    {
        Response response = new Response();
        ConnectManager connectManager = new ConnectManager();
        SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice"));
        response = connectManager.register(manager, connection);
        return response;
    }
    
    [HttpPost]
    [Route("Login")]
    public Response login(Login login)
    {
        Response response = new Response();
        ConnectManager connectManager = new ConnectManager();
        SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice"));
        response = connectManager.Login(login, connection);
        return response;
    }
    
    [HttpGet]
    [Route("getStaffAll")]
    public Response getStaffAll()
    {
        Response response = new Response();
        ConnectManager connectManager = new ConnectManager();
        SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice"));
        response = connectManager.getStaffAll(connection);
        return response;
    }
    
    [HttpDelete]
    [Route("deleteStaff")]
    public Response deleteStaff(int managerId)
    {
        Response response = new Response();
        ConnectManager connectManager = new ConnectManager();
        SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice"));
        response = connectManager.delStaff(connection, managerId);
        return response;
    }

    [HttpPatch]
    [Route("updateInfoStaff")]
    public Response updateInfoStaff(Managers managers)
    {
        Response response = new Response();
        ConnectManager connectManager = new ConnectManager();
        SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice"));
        response = connectManager.updateInfoStaff(managers, connection);
        return response;
    }
    
    [HttpPatch]
    [Route("adminUpdateInfoStaff")]
    public Response adminUpdateInfoStaff(Managers managers)
    {
        Response response = new Response();
        ConnectManager connectManager = new ConnectManager();
        SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice"));
        response = connectManager.adminUpdateInfoStaff(managers, connection);
        return response;
    }

    [HttpPatch]
    [Route("resetPassword")]
    public Response resetPassword(int managerId)
    {
        Response response = new Response();
        ConnectManager connectManager = new ConnectManager();
        SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice"));
        response = connectManager.resetPassword(connection, managerId);
        return response;
    }
    
    [HttpGet]
    [Route("getStaffById")]
    public Response getStaffById(int managerId)
    {
        Response response = new Response();
        ConnectManager connectManager = new ConnectManager();
        SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("webservice"));
        response = connectManager.getStaffInfoById(connection, managerId);
        return response;
    }
}