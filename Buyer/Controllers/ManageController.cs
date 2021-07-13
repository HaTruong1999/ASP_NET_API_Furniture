using Buyer.Mvc.Models.Producer;
using Buyer.Mvc.Models.Product;
using Buyer.Mvc.Models.Room;
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
	public class ManageController : Controller
	{
        public string idInsert { get; set; }
        public string idErr { get; set; }
        public bool enableFormAddProduct { get; set; } = false;
        public string userMain { get; set; }
		public new User User { get; set; }
		public List<User> Users { get; set; } = new List<User>();
		public List<Product> Products { get; set; } = new List<Product>();
        public List<Producer> Producers { get; set; } = new List<Producer>();
        public List<Room> Rooms { get; set; } = new List<Room>();
        //product
        public IActionResult Index(bool EnableFormAddProduct, string IdInsert, string IdErr)
		{
            userMain = HttpContext.Session.GetString("UserMain").ToString();
            if (EnableFormAddProduct)
                enableFormAddProduct = EnableFormAddProduct;

            if (!string.IsNullOrEmpty(userMain))
            {
                string BaseUrl = "http://localhost:3000/users/" + userMain.Trim();
                User = JsonConvert.DeserializeObject<List<User>>(getdata(BaseUrl))[0];
            }

            if (!string.IsNullOrEmpty(IdErr))
                idErr = IdErr;

            if (!string.IsNullOrEmpty(IdInsert))
                idInsert = IdInsert;

            GetProducts();
            GetProducers();
            GetRooms();
            var model = (Products,Producers,Rooms,User, enableFormAddProduct,idInsert,idErr);
            return View(model);
		}
        public bool CheckExistProduct(string id)
		{
            foreach (Product p in Products)
                if (p.id.Trim().Equals(id))
                    return false;
            return true;
		}
        [HttpPost]
        public IActionResult InsertProduct(string id , string name , float price, float oldPrice, string photo, int amount, string roomId, string producerId)
        {
            GetProducts();
			if (CheckExistProduct(id.Trim()))
			{
               return  RedirectToAction("Index", new { EnableFormAddProduct = false });
            }

            return RedirectToAction("Index", new { EnableFormAddProduct = true,IdInsert = id, IdErr = "Mã sản phẩm đã tồn tại!" });
        }
        public IActionResult ManageUser()
        {
            userMain = HttpContext.Session.GetString("UserMain").ToString();
            if (!string.IsNullOrEmpty(userMain))
            {
                string BaseUrl = "http://localhost:3000/users/" + userMain.Trim();
                User = JsonConvert.DeserializeObject<List<User>>(getdata(BaseUrl))[0];
            }
            GetUsers();
            var model = (Users, User);
            return View(model);
        }
        public void GetProducts()
        {
            string BaseUrl = "http://localhost:3000/products";
            Products = JsonConvert.DeserializeObject<List<Product>>(getdata(BaseUrl));
        }
        public void GetUsers()
        {
            string BaseUrl = "http://localhost:3000/users";
            Users = JsonConvert.DeserializeObject<List<User>>(getdata(BaseUrl));
        }
        public void GetProducers()
        {
            string BaseUrl = "http://localhost:3000/producers";
            Producers = JsonConvert.DeserializeObject<List<Producer>>(getdata(BaseUrl));
        }
        public void GetRooms()
        {
            string BaseUrl = "http://localhost:3000/rooms";
            Rooms = JsonConvert.DeserializeObject<List<Room>>(getdata(BaseUrl));
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
