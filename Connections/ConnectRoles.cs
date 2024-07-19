using System.Data;
using Microsoft.Data.SqlClient;
using WebServiceShopping.Models;

namespace WebServiceShopping.Connections;

public class ConnectRoles
{
    public Response getRoles(SqlConnection connection)
    {
        Response response = new Response();
        try
        {
            connection.Open();
            string query = "SELECT * FROM roles";
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataAdapter adapter = new SqlDataAdapter(command);

            DataTable dataTable = new DataTable();
            
            adapter.Fill(dataTable);
            connection.Close();
            List<Roles> roles = new List<Roles>();
            if (dataTable.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Roles role = new Roles();
                    role.roleId = Convert.ToInt32(dataTable.Rows[i]["roleId"]);
                    role.roleName = Convert.ToString(dataTable.Rows[i]["roleName"]);
                    roles.Add(role);
                }
            }
            if (roles.Count > 0)
            {
                response.StatusCode = 200;
                response.StatusMessage = "Danh sách tất cả role";
                response.roles = roles;
            }
        }
        catch (Exception e)
        {
            response.StatusCode = 400;
            response.StatusMessage = e.Message;
        }
        finally
        {
            connection.Close();
        }
        return response;
    }
}