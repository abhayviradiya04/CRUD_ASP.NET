using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using CRUD_Demo.Models;

namespace CRUD_Demo.Controllers
{
    public class ProductController : Controller
    {
        private IConfiguration configuration;

        public ProductController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public IActionResult Index()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "sp_GetAllProducts";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }

        public IActionResult ProductDelete(int ProductID)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "sp_DeleteProduct";
                command.Parameters.Add("@ProductID", SqlDbType.Int).Value = ProductID;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("Index");
        }

            public IActionResult ProductSave(ProductModel productModel)
            {
            

                if (ModelState.IsValid)
                {
                    string connectionString = this.configuration.GetConnectionString("ConnectionString");
                    SqlConnection connection = new SqlConnection(connectionString);
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    if (productModel.ProductID == null)
                    {
                        command.CommandText = "sp_InsertProduct";
                    }
                    else
                    {
                        command.CommandText = "sp_UpdateProduct";
                        command.Parameters.Add("@ProductID", SqlDbType.Int).Value = productModel.ProductID;
                    }
                    command.Parameters.Add("@Name", SqlDbType.VarChar).Value = productModel.Name;
                    command.Parameters.Add("@Price", SqlDbType.Decimal).Value = productModel.Price;
                    command.ExecuteNonQuery();
                    return RedirectToAction("Index");
                }

                return View("ProductAddEdit", productModel);
            }

        public IActionResult ProductAddEdit(int? ProductID)
        {
            // If adding a new product (ProductID is null)
            if (ProductID == null)
            {
                return View("ProductAddEdit", new ProductModel()); // Send an empty model to avoid null reference
            }

            ProductModel productModel = new ProductModel();
            string connectionString = this.configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "sp_GetProductById";
                    command.Parameters.AddWithValue("@ProductID", ProductID);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            productModel.ProductID = Convert.ToInt32(reader["ProductID"]);
                            productModel.Name = reader["Name"].ToString();
                            productModel.Price = Convert.ToDecimal(reader["Price"]);
                        }
                    }
                }
            }

            return View("ProductAddEdit", productModel);
        }

    }
}
