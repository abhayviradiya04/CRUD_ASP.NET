
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using CRUD_Demo_Practise.Models;


namespace CRUD_Demo_Practise.Controllers
{
    public class ProductController : Controller
    {
        private IConfiguration configuration;
            
        public ProductController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        #region SelectAll

        public IActionResult View()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection (connectionString);
            connection.Open ();
            SqlCommand command = connection.CreateCommand();
            command.CommandType=CommandType.StoredProcedure;
            command.CommandText = "PR_Prosuct_SelectAll";
            SqlDataReader reader= command.ExecuteReader();
            DataTable db=new DataTable();
            db.Load (reader);

            return View(db);
        }
        #endregion

        #region SelectByPk
        public IActionResult ProductBySelectPk(int productId)
        {
            ProductModel product = new ProductModel();
            string connectionString = this.configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("PR_Prosuct_SelectByPk", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ProductId", productId);

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    product.ProductId = Convert.ToInt32(reader["ProductId"]);
                    product.Name = reader["Name"].ToString();
                    product.Quantity = Convert.ToInt32(reader["Quantity"]);
                }
            }

            return View("AddEdit", product);
        }

        #endregion

        #region Insert/Update
        // Add or Edit View
        public IActionResult AddEdit(int? ProductID)
        {
            if (ProductID != null)
            {
                return RedirectToAction("ProductBySelectPk", new { productId = ProductID });
            }

            return View(new ProductModel()); // 👈 Pass an empty model

        }


        // Save (Insert/Update)
        [HttpPost]
        public IActionResult Save(ProductModel product)
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;

                    if (product.ProductId == 0)
                    {
                        command.CommandText = "PR_Products_Add";
                        command.Parameters.AddWithValue("@ProductName", product.Name);     // ✅
                        command.Parameters.AddWithValue("@Quantity", product.Quantity);
                    }
                    else
                    {
                        command.CommandText = "PR_Products_Update";
                        command.Parameters.AddWithValue("@ProductID", product.ProductId);  // ✅ matches SP param
                        command.Parameters.AddWithValue("@ProductName", product.Name);     // ✅ matches SP param
                        command.Parameters.AddWithValue("@Quantity", product.Quantity);
                    }

                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("View");
        }

        #endregion 

        // Delete
        public IActionResult Delete(int ProductID)
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Products_Delete";
            command.Parameters.AddWithValue("@ProductID", ProductID);
            command.ExecuteNonQuery();
            
            return RedirectToAction("View");
        }
    }
}
