using Buyer.Mvc.Models.DetailOrder;
using Buyer.Mvc.Models.Order;
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
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Buyer.Mvc.Controllers
{
	public class ManageController : Controller
	{
        public string idInsert { get; set; }
        public string idErr { get; set; }
        public bool enableFormAddProduct { get; set; } = false;
        public bool deleteErr { get; set; } = false;
        public string userMain { get; set; }
		public new User User { get; set; }
		public List<User> Users { get; set; } = new List<User>();
		public List<Product> Products { get; set; } = new List<Product>();
        public List<Producer> Producers { get; set; } = new List<Producer>();
        public List<Room> Rooms { get; set; } = new List<Room>();
        public List<DetailOrder> DetailOrders { get; set; } = new List<DetailOrder>();
        //product
        public async Task<IActionResult> IndexAsync(bool EnableFormAddProduct, string IdInsert, string IdErr, bool DeleteErr)
		{
            userMain = HttpContext.Session.GetString("UserMain").ToString();
            if (EnableFormAddProduct)
                enableFormAddProduct = EnableFormAddProduct;
            if (DeleteErr)
                deleteErr = DeleteErr;

            if (!string.IsNullOrEmpty(userMain))
            {
                string BaseUrl = "http://localhost:3000/users/" + userMain.Trim();
                User = JsonConvert.DeserializeObject<List<User>>(getdata(BaseUrl))[0];
            }

            if (!string.IsNullOrEmpty(IdErr))
                idErr = IdErr;

            if (!string.IsNullOrEmpty(IdInsert))
                idInsert = IdInsert;

            await GetProductsAsync();
            await GetProducersAsync();
            await GetRoomsAsync();
            var model = (Products,Producers,Rooms,User, enableFormAddProduct,idInsert,idErr,deleteErr);
            return View(model);
		}
        public bool CheckExistProduct(string id)
		{
            foreach (Product p in Products)
                if (p.id.Trim().Equals(id))
                    return false;
            return true;
		}
        public async Task CallApiCreateProductAsync(String baseUrl, Product Product)
        {
            var httpClient = new HttpClient();

            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = HttpMethod.Post;
            httpRequestMessage.RequestUri = new Uri(baseUrl);

            // Tạo StringContent
            string jsoncontent = JsonConvert.SerializeObject(Product);
            var httpContent = new StringContent(jsoncontent, Encoding.UTF8, "application/json");
            httpRequestMessage.Content = httpContent;

            var response = await httpClient.SendAsync(httpRequestMessage);
            var responseContent = await response.Content.ReadAsStringAsync();
        }
        [HttpPost]
        public async Task<IActionResult> InsertProductAsync(string id , string name , float price, float oldPrice, string photo, int amount, string roomId, string producerId)
        {
            await GetProductsAsync();
			if (CheckExistProduct(id.Trim()))
			{
                Product product = new Product(id, name, price, oldPrice, photo, amount, roomId, producerId);
                string BaseUrl = "http://localhost:3000/products";
				_ = CallApiCreateProductAsync(BaseUrl, product);
                return  RedirectToAction("Index", new { EnableFormAddProduct = false });
            }

            return RedirectToAction("Index", new { EnableFormAddProduct = true,IdInsert = id, IdErr = "Mã sản phẩm đã tồn tại!" });
        }
        public bool CheckExistDetailOrder(string id)
        {
            foreach (DetailOrder d in DetailOrders)
                if (d.productId.Trim().Equals(id))
                    return false;
            return true;
        }
        public async Task CallApiDeleteProductAsync(String baseUrl)
        {
            var httpClient = new HttpClient();

            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = HttpMethod.Delete;
            httpRequestMessage.RequestUri = new Uri(baseUrl);

            var response = await httpClient.SendAsync(httpRequestMessage);
            var responseContent = await response.Content.ReadAsStringAsync();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteProductAsync(string id)
        {
            string BaseUrl;
            await GetDetailOrdersAsync();
			if (DetailOrders.Count() > 0)
			{
                if (CheckExistDetailOrder(id))
                {
                    BaseUrl = "http://localhost:3000/products/" + id.Trim();
                    _ = CallApiDeleteProductAsync(BaseUrl);
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index", new {DeleteErr = true});
            }

            BaseUrl = "http://localhost:3000/products/" + id.Trim();
            _ = CallApiDeleteProductAsync(BaseUrl);
            return RedirectToAction("Index");
        }
        public async Task CallApiEditProductAsync(String baseUrl, Product Product)
        {
            var httpClient = new HttpClient();

            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = HttpMethod.Put;
            httpRequestMessage.RequestUri = new Uri(baseUrl);

            // Tạo StringContent
            string jsoncontent = JsonConvert.SerializeObject(Product);
            var httpContent = new StringContent(jsoncontent, Encoding.UTF8, "application/json");
            httpRequestMessage.Content = httpContent;

            var response = await httpClient.SendAsync(httpRequestMessage);
            var responseContent = await response.Content.ReadAsStringAsync();
        }
        [HttpPost]
        public IActionResult EditProduct(string id, string name, float price, float oldPrice, string photo, int amount, string roomId, string producerId)
        {
            Product product = new Product(id, name, price, oldPrice, photo, amount, roomId, producerId);
            string BaseUrl = "http://localhost:3000/products/"+ id;
            _ = CallApiEditProductAsync(BaseUrl, product);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> ManageUserAsync()
        {
            userMain = HttpContext.Session.GetString("UserMain").ToString();
            if (!string.IsNullOrEmpty(userMain))
            {
                string BaseUrl = "http://localhost:3000/users/" + userMain.Trim();
                User = JsonConvert.DeserializeObject<List<User>>(getdata(BaseUrl))[0];
            }
            await GetUsersAsync();
            var model = (Users, User);
            return View(model);
        }
        public async Task GetProductsAsync()
        {
            string BaseUrl = "http://localhost:3000/products";
            //Products =  JsonConvert.DeserializeObject<List<Product>>(getdata(BaseUrl));
            Task<string> datatask = CallApiGetData(BaseUrl);
            Products = JsonConvert.DeserializeObject<List<Product>>(await datatask);
        }
        public async Task GetUsersAsync()
        {
            string BaseUrl = "http://localhost:3000/users";
            //Users = JsonConvert.DeserializeObject<List<User>>(getdata(BaseUrl));
            Task<string> datatask = CallApiGetData(BaseUrl);
            Users = JsonConvert.DeserializeObject<List<User>>(await datatask);
        }
        public async Task GetProducersAsync()
        {
            string BaseUrl = "http://localhost:3000/producers";
            //Producers = JsonConvert.DeserializeObject<List<Producer>>(getdata(BaseUrl));
            Task<string> datatask = CallApiGetData(BaseUrl);
            Producers = JsonConvert.DeserializeObject<List<Producer>>(await datatask);
        }
        public async Task GetRoomsAsync()
        {
            string BaseUrl = "http://localhost:3000/rooms";
            //Rooms = JsonConvert.DeserializeObject<List<Room>>(getdata(BaseUrl));
            Task<string> datatask = CallApiGetData(BaseUrl);
            Rooms = JsonConvert.DeserializeObject<List<Room>>(await datatask);
        }
        public async Task GetDetailOrdersAsync()
        {
            string BaseUrl = "http://localhost:3000/OrderDetails";
            //DetailOrders = JsonConvert.DeserializeObject<List<DetailOrder>>(getdata(BaseUrl));
            Task<string> datatask = CallApiGetData(BaseUrl);
            DetailOrders = JsonConvert.DeserializeObject<List<DetailOrder>>(await datatask);
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
        public async Task<string> CallApiGetData(String baseUrl)
        {
            var httpClient = new HttpClient();

            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = HttpMethod.Get;
            httpRequestMessage.RequestUri = new Uri(baseUrl);

            // Tạo StringContent
            //string jsoncontent = JsonConvert.SerializeObject(Order);
            var httpContent = new StringContent("", Encoding.UTF8, "application/json");
            httpRequestMessage.Content = httpContent;

            var response = await httpClient.SendAsync(httpRequestMessage);
            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }
    }
}
