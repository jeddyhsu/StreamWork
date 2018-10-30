using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StreamWork.Models;
using System.Data;
using System.Data.SqlClient;
namespace StreamWork.Controllers
{
    public class HomeController : Controller
    {
        String connectionString = "Data Source=RITHVIK-LAPTOP\\RITHVIKSSQL;Initial Catalog=StreamWorkSignIn;Integrated Security=True";
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Math()
        {
            return View();
        }
        public IActionResult Literature()
        {
            return View();
        }
        public IActionResult Engineering()
        {
            return View();
        }
        public IActionResult DesignArt()
        {
            return View();
        }
        public IActionResult Science()
        {
           
            return View();
        }
        public IActionResult Business()
        {
            return View();
        }
        public IActionResult Programming()
        {
            return View();
        }
        public IActionResult Other()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SignUp(string username, string password, string confirmpassword)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            String a="";
            
            
            if (connection.State == ConnectionState.Open)
            {
                
                    SqlCommand sqlCommand = new SqlCommand("SELECT COUNT(*) FROM SignIN WHERE Username = @Username", connection);
                    sqlCommand.Parameters.AddWithValue("@Username", username);
                    int num = (int)sqlCommand.ExecuteScalar();
                    if (num > 0)
                    {
                    Console.WriteLine("Username already exsists");
                    a = "Error";
                    }
                else 
                {
                    {
                        if (password.Equals(confirmpassword))
                        {
                            String query = "INSERT INTO SignIN(Password,Username)";
                            query += "VALUES (@Password,@Username)";
                            SqlCommand cmd = new SqlCommand(query, connection);
                            cmd.Parameters.AddWithValue("@Username", username);
                            cmd.Parameters.AddWithValue("@Password", password);
                            cmd.ExecuteNonQuery();
                            a = "Sucsess";
                        }
                        else
                        {
                            a = "Wrong confirmPassword";
                        }
                       
                    }

                }



            }


            return Json(new { Message = a });

        }
        [HttpGet]
        public IActionResult SignUp()
        {

            return View();
        }
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            String forJson = "";
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            String loginQuery = "SELECT COUNT(*) FROM SignIN WHERE Username = @Username AND Password = @Password";
            SqlCommand loginCommand = new SqlCommand(loginQuery, connection);
            loginCommand.Parameters.AddWithValue("@Username", username);
            loginCommand.Parameters.AddWithValue("@Password", password);

           int val = (int) loginCommand.ExecuteScalar();

            if(val > 0)
            {
                forJson = "Welcome";
            }
            else
            {
                forJson = "Error";
            }

            return Json(new { Message = forJson });

        }

        [HttpGet]
        public IActionResult Login()
        {
          
            return View();

        }

        public IActionResult Profile()
        {
            return View();
        }
       
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
