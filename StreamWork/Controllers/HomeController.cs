using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StreamWork.Models;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Microsoft.AspNetCore.Http;
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
        public IActionResult SignUp(string nameFirst, string nameLast, string email, int phone, string username, string password, string passwordConfirm)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            String a="";
            
            
            if (connection.State == ConnectionState.Open)
            {
                
                    SqlCommand sqlCommand = new SqlCommand("SELECT COUNT(*) FROM SignUp WHERE Username = @Username", connection);
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
                        if (password.Equals(passwordConfirm))
                        {
                            String query = "INSERT INTO SignUp(FirstName,LastName,Email,PhoneNumber,Username,Password)";
                            query += "VALUES (@FirstName,@LastName,@Email,@PhoneNumber,@Username,@Password)";
                            SqlCommand cmd = new SqlCommand(query, connection);
                            cmd.Parameters.AddWithValue("@FirstName", nameFirst);
                            cmd.Parameters.AddWithValue("@LastName", nameLast);
                            cmd.Parameters.AddWithValue("@Email", email);
                            cmd.Parameters.AddWithValue("@PhoneNumber", phone);
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

            String loginQuery = "SELECT COUNT(*) FROM SignUp WHERE Username = @Username AND Password = @Password";
            SqlCommand loginCommand = new SqlCommand(loginQuery, connection);
            loginCommand.Parameters.AddWithValue("@Username", username);
            loginCommand.Parameters.AddWithValue("@Password", password);
            
           int val = (int) loginCommand.ExecuteScalar();

            if(val > 0)
            {
                HttpContext.Session.SetString("UserProfile", username);
                
                
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
            var model = new UserProfile();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            var user = HttpContext.Session.GetString("UserProfile");
            String loginQuery = "SELECT FirstName, LastName FROM SignUp WHERE Username = @Username";
            SqlCommand loginCommand = new SqlCommand(loginQuery, connection);
            loginCommand.Parameters.AddWithValue("@Username", user);
            var reader = loginCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {

                    model.FirstName = reader.GetString(0);
                    model.LastName = reader.GetString(1);
                }
            }
           
            return View(model);
        }
       
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
