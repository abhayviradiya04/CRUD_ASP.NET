// Controller: CustomerController.cs
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using CRUD_Demo_Practise.Models;

namespace CRUD_Demo_Practise.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IConfiguration configuration;

        public CustomerController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public IActionResult View()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = new SqlCommand("Pr_Customers_SelectAll", connection);
            command.CommandType = CommandType.StoredProcedure;
            SqlDataReader reader = command.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(reader);
            return View(dt);
        }

        [HttpGet]
        public IActionResult AddOrEdit(int? id)
        {
            if (id == null)
                return View(new CustomerModel());

            string connectionString = configuration.GetConnectionString("ConnectionString");
            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = new SqlCommand("Pr_Customers_SelectByPK", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@CustomerId", id);
            SqlDataReader reader = command.ExecuteReader();
            CustomerModel model = new CustomerModel();
            if (reader.Read())
            {
                model.CustomerId = Convert.ToInt32(reader["CustomerId"]);
                model.CustomerName = reader["CustomerName"].ToString();
                model.ProductId = Convert.ToInt32(reader["ProductID"]);
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Save(CustomerModel model)
        {
            if (!ModelState.IsValid)
                return View("AddOrEdit", model);

            string connectionString = configuration.GetConnectionString("ConnectionString");
            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = new SqlCommand(model.CustomerId == 0 ? "Pr_Customer_Insert" : "Pr_Customers_Update", connection);
            command.CommandType = CommandType.StoredProcedure;

            if (model.CustomerId != 0)
                command.Parameters.AddWithValue("@CustomerId", model.CustomerId);

            command.Parameters.AddWithValue("@CustomerName", model.CustomerName);
            command.Parameters.AddWithValue("@ProductID", model.ProductId);
            command.ExecuteNonQuery();

            return RedirectToAction("View");
        }

        public IActionResult Delete(int id)
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = new SqlCommand("Pr_Customers_Delete", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@CustomerId", id);
            command.ExecuteNonQuery();
            return RedirectToAction("View");
        }
    }
}
