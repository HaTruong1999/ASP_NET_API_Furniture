using Buyer.Mvc.Models.DetailOrder;
using Buyer.Mvc.Models.Order;
using Buyer.Mvc.Models.Product;
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
        public List<Order> Orders { get; set; } = new List<Order>();
        public List<DetailOrder> DetailOrders { get; set; } = new List<DetailOrder>();

        public string BaseUrl = "";
		public async Task<IActionResult> IndexAsync(string userMain,string userName, string userNameErr, string password, string passwordErr, bool loginFailure)
		{
            User = new User();
            if (!string.IsNullOrEmpty(userMain))
			{
                string BaseUrl = "http://localhost:3000/users/" + userMain.Trim();
                User = JsonConvert.DeserializeObject<List<User>>(getdata(BaseUrl))[0];

                await GetOrdersAsync(userMain);
                int tempOrderId = FindOrderID();
                if (tempOrderId > 0)
                    await GetDetailOrdersAsync (tempOrderId);
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

            await GetProductsAsync();
            var model = (DetailOrders,Products_guestroom, Products_bedroom, Products_chickenroom, User, UserName, UserNameErr, Password, PasswordErr , LoginFailure);
            return View(model);
		}
        [HttpPost]
        public async Task<IActionResult> LoginToPageAsync(string Username, string Password)
        {
            await GetUsersAsync();
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
        public async Task GetUsersAsync()
		{
            string BaseUrl = "http://localhost:3000/users";
            //Users = JsonConvert.DeserializeObject<List<User>>(getdata(BaseUrl));
            Task<string> datatask = CallApiGetData(BaseUrl);
            Users = JsonConvert.DeserializeObject<List<User>>(await datatask);
		}
        public async Task GetProductsAsync()
        {
            string BaseUrl = "http://localhost:3000/products";
            //Products = JsonConvert.DeserializeObject<List<Product>>(getdata(BaseUrl));
            Task<string> datatask = CallApiGetData(BaseUrl);
            Products = JsonConvert.DeserializeObject<List<Product>>(await datatask);

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
        public async Task GetOrdersAsync(string userId)
        {
            string BaseUrl = "http://localhost:3000/orders/users/" + userId;
            //Orders = JsonConvert.DeserializeObject<List<Order>>(getdata(BaseUrl));
            Task<string> datatask = CallApiGetData(BaseUrl);
            Orders = JsonConvert.DeserializeObject<List<Order>>(await datatask);
        }
        public async Task GetAllOrdersAsync()
        {
            string BaseUrl = "http://localhost:3000/orders";
            //Orders = JsonConvert.DeserializeObject<List<Order>>(getdata(BaseUrl));
            Task<string> datatask = CallApiGetData(BaseUrl);
            Orders = JsonConvert.DeserializeObject<List<Order>>(await datatask);
        }
        public async Task GetDetailOrdersAsync(int orderId)
        {
            string BaseUrl = "http://localhost:3000/OrderDetails/"+ orderId;
            //DetailOrders = JsonConvert.DeserializeObject<List<DetailOrder>>(getdata(BaseUrl));
            Task<string> datatask = CallApiGetData(BaseUrl);
            DetailOrders = JsonConvert.DeserializeObject<List<DetailOrder>>(await datatask);
        }
        //Tim Order ID
        public int FindOrderID()
		{
            foreach(Order order in Orders)
			{
                if (!order.bought)
                    return order.id;
			}
            return 0;
		}
        public async Task CallApiInsertOrder(String baseUrl, Order Order)
        {
            var httpClient = new HttpClient();

            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = HttpMethod.Post;
            httpRequestMessage.RequestUri = new Uri(baseUrl);

            // Tạo StringContent
            string jsoncontent = JsonConvert.SerializeObject(Order);
            var httpContent = new StringContent(jsoncontent, Encoding.UTF8, "application/json");
            httpRequestMessage.Content = httpContent;

            var response = await httpClient.SendAsync(httpRequestMessage);
            var responseContent = await response.Content.ReadAsStringAsync();
        }
        public int FindOrderIDToAddOrder(string userId)
        {
            int temp = 0;
            foreach (Order order in Orders)
            {
                if (order.id > temp)
                    temp = order.id;
                if (!order.bought && order.userId.Trim().Equals(userId))
                    return order.id;
            }
            //create new order
            Order newOrder = new Order(temp + 10, userId);
            string BaseUrl = "http://localhost:3000/orders";
            _ = CallApiInsertOrder(BaseUrl, newOrder);

            return temp + 10;
        }
        //CALL API
        public string getdata(string baseUrl)
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
        //Add to Cart
        public async Task CallApiInsertOrderDetail(String baseUrl, DetailOrder detailOrder)
        {
            var httpClient = new HttpClient();

            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = HttpMethod.Post;
            httpRequestMessage.RequestUri = new Uri(baseUrl);

            // Tạo StringContent
            string jsoncontent = JsonConvert.SerializeObject(detailOrder);
            var httpContent = new StringContent(jsoncontent, Encoding.UTF8, "application/json");
            httpRequestMessage.Content = httpContent;

            var response = await httpClient.SendAsync(httpRequestMessage);
            var responseContent = await response.Content.ReadAsStringAsync();
        }
        public void InsertOrderDetail(DetailOrder detailOrder)
        {
            string BaseUrl = "http://localhost:3000/OrderDetails";
            _ = CallApiInsertOrderDetail(BaseUrl, detailOrder);
        }
        public async Task CallApiUpdateOrderDetail(String baseUrl, DetailOrder detailOrder)
        {
            var httpClient = new HttpClient();

            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = HttpMethod.Put;
            httpRequestMessage.RequestUri = new Uri(baseUrl);

            // Tạo StringContent
            string jsoncontent = JsonConvert.SerializeObject(detailOrder);
            var httpContent = new StringContent(jsoncontent, Encoding.UTF8, "application/json");
            httpRequestMessage.Content = httpContent;

            var response = await httpClient.SendAsync(httpRequestMessage);
            var responseContent = await response.Content.ReadAsStringAsync();
        }
        public void UpdateOrderDetail(DetailOrder detailOrder)
        {
            string BaseUrl = "http://localhost:3000/OrderDetails/"+ detailOrder.id;
            _ = CallApiUpdateOrderDetail(BaseUrl, detailOrder);
        }
        public async Task<DetailOrder> CheckExitsProductAsync(int orderId, string productId)
		{
            await GetDetailOrdersAsync(orderId);
            foreach (DetailOrder d in DetailOrders)
                if (d.productId.Trim().Equals(productId))
                    return d;
            return new DetailOrder();
        }
        public async Task<IActionResult> AddToCartAsync(string Id)
        {
            string tempUserId = HttpContext.Session.GetString("UserMain");
            await GetAllOrdersAsync();
            int orderId = FindOrderIDToAddOrder(tempUserId);

            DetailOrder tempDetailOrder = await CheckExitsProductAsync(orderId, Id);
            if (tempDetailOrder.quantity > 0)
			{
                tempDetailOrder.quantity = tempDetailOrder.quantity + 1;
                UpdateOrderDetail(tempDetailOrder);
                return RedirectToAction("Index", new { userMain = tempUserId });
            }
            DetailOrder detailOrder = new(0, orderId, Id.Trim(), 1);

            InsertOrderDetail(detailOrder);
            return RedirectToAction("Index", new { userMain = tempUserId });
        }
        //Confirm Order
        public async Task CallApiConfirmOrder(String baseUrl)
        {
            var httpClient = new HttpClient();

            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = HttpMethod.Put;
            httpRequestMessage.RequestUri = new Uri(baseUrl);

            var response = await httpClient.SendAsync(httpRequestMessage);
            var responseContent = await response.Content.ReadAsStringAsync();
        }
        public IActionResult ConfirmOrder(int OrderId)
        {
            string tempUserId = HttpContext.Session.GetString("UserMain");

            string BaseUrl = "http://localhost:3000/orders/" + OrderId;
            _ = CallApiConfirmOrder(BaseUrl);

            return RedirectToAction("Index", new { userMain = tempUserId });
        }
    }
}
