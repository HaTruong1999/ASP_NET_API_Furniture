using Buyer.Mvc.Models.Product;
using Buyer.Mvc.Models.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buyer.Mvc.Controllers
{
	public class PageController : Controller
	{
        public string UserName { get; set; }
        public string UserNameErr { get; set; } = "";
        public string Password { get; set; }
        public string PasswordErr { get; set; } = "";
        public bool LoginFailure { get; set; } = false;
        public new User User { get; set; }
        public List<User> Users { get; set; } = new List<User>();
        public List<Product> Products { get; set; } = new List<Product>();
		public List<Product> Products_guestroom { get; set; } = new List<Product>();
		public List<Product> Products_bedroom { get; set; } = new List<Product>();
		public List<Product> Products_chickenroom { get; set; } = new List<Product>();

		public string BaseUrl = "";
		public IActionResult Index(string userMain,string userName, string userNameErr, string password, string passwordErr, bool loginFailure)
		{
            User = new User();
            if (!string.IsNullOrEmpty(userMain))
			{
                string BaseUrl = "http://localhost:3000/users/" + userMain.Trim();
                User = JsonConvert.DeserializeObject<List<User>>(getdata(BaseUrl))[0];
            }
            if (!string.IsNullOrEmpty(userName))
                UserName = userName;
            if (!string.IsNullOrEmpty(userNameErr))
                UserNameErr = userNameErr;
            if (!string.IsNullOrEmpty(password))
                Password = password;
            if (!string.IsNullOrEmpty(passwordErr))
                PasswordErr = passwordErr;
            if (loginFailure)
                LoginFailure = loginFailure;

            GetProducts();
            var model = (Products_guestroom, Products_bedroom, Products_chickenroom, User, UserName, UserNameErr, Password, PasswordErr , LoginFailure);
            return View(model);
		}
        [HttpPost]
        public IActionResult LoginToPage(string Username, string Password)
        {
            GetUsers();
            foreach (User user in Users)
            {
                if (user.username.Trim().Equals(Username.Trim()))
                {
                    if (user.password.Trim().Equals(Password.Trim()))
                    {
                        HttpContext.Session.SetString("UserMain", user.username.Trim().ToString());
                        return RedirectToAction("Index", new { userMain = Username });
                    }
                    return RedirectToAction("Index", new { passwordErr = "Incorreact password!" });
                }
            }
            return RedirectToAction("Index", new { userName = Username, userNameErr = "User is not exist!"});
        }
        public void GetUsers()
		{
            string BaseUrl = "http://localhost:3000/users";
            Users = JsonConvert.DeserializeObject<List<User>>(getdata(BaseUrl));
        }
        public void GetProducts()
        {
            string BaseUrl = "http://localhost:3000/products";
            Products = JsonConvert.DeserializeObject<List<Product>>(getdata(BaseUrl));
            foreach (Product p in Products)
            {
                if (p.roomId.Equals("BR003     "))
                {
                    Products_bedroom.Add(p);
                }
                else if (p.roomId.Equals("CR002     "))
                {
                    Products_chickenroom.Add(p);
                }
                else
                {
                    Products_guestroom.Add(p);
                }
            }
        }
        //CALL API
        public string getdata(String baseUrl)
        {
            string json = null;
            try
            {
                Uri baseURL = new Uri(baseUrl);
                using (var webClient = new System.Net.WebClient())
                {
                    json = webClient.DownloadString(baseURL);
                }
            }
            catch (ArgumentNullException uex)
            {
                RedirectToPage("error", new { msg = uex.Message + " | url missing or invalid." });
            }
            catch (Exception ex)
            {
                RedirectToPage("error", new { msg = ex.Message + " | are you missing some json keys and values? please check your json data." });
            }
            return json;
        }
    }
}
