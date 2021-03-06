using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Buyer.Mvc.Models;
using Buyer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Buyer.Mvc.Models.Order;
using Newtonsoft.Json;
using System;
using Buyer.Mvc.Models.User;
using Buyer.Mvc.Models.DetailOrder;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;

namespace Buyer.Mvc.Controllers
{
    public class HistoryController : Controller
    {
        public string UserID { get; set; }
        public string CustID { get; set; }
        private DatabaseContext Context { get; }
        public HistoryController(DatabaseContext _db)
        {
            Context = _db;
        }
        public List<OrderHeader> ListOrderHeader = new List<OrderHeader>();
        //Detail Order Line
        public OrderHeader OrderHeader = new OrderHeader();
        public List<OrderLine> ListOrderLine = new List<OrderLine>();
        //furniture
        public string UserMain { get; set; }
        public User User{ get; set; } = new User();
        public List<User> Users { get; set; } = new List<User>();
        public Order Order { get; set; } = new Order();
        public List<Order> Orders { get; set; } = new List<Order>();
        public List<DetailOrder> DetailOrders { get; set; } = new List<DetailOrder>();
        public void viewLists()
        {
            ListOrderHeader = Context.OrderHeaderDbs.FromSqlRaw("dbo.[usp_Order] @p0, @p1,@p2", "ViewOrderHeader", 0, CustID).ToList();
        }
        public async Task<IActionResult> IndexAsync()
        {
            UserMain = HttpContext.Session.GetString("UserMain").ToString();
            string BaseUrl = "http://localhost:3000/users/" + UserMain.Trim();
            User = JsonConvert.DeserializeObject<List<User>>(getdata(BaseUrl))[0];
            if (User.admin)
                await GetOrdersAsync();
            else
                await GetOrdersAsync(UserMain);
            var model = (Orders, User);
            return View(model);
        }
        public async Task<IActionResult> DetailOrderAsync(int id)
        {
            UserMain = HttpContext.Session.GetString("UserMain").ToString();

            string BaseUrl = "http://localhost:3000/users/" + UserMain.Trim();
            //User = JsonConvert.DeserializeObject<List<User>>(getdata(BaseUrl))[0];
            Task<string> datatask = CallApiGetData(BaseUrl);
            User = JsonConvert.DeserializeObject<List<User>>(await datatask)[0];

            await GetOrderAsync(id);
            await GetDetailOrdersAsync(id);
            int totalQuantity = 0;
            foreach (DetailOrder o in DetailOrders)
			{
                totalQuantity = totalQuantity + o.quantity;
            }
               
            var model = (Order,DetailOrders,User, totalQuantity);
            return View(model);
        }
        public async Task CallApiUpdateOrderAsync(String baseUrl)
        {
            var httpClient = new HttpClient();

            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = HttpMethod.Put;
            httpRequestMessage.RequestUri = new Uri(baseUrl);

            var response = await httpClient.SendAsync(httpRequestMessage);
        }
        public IActionResult CancelOrder(int id)
        {
            string BaseUrl = "http://localhost:3000/CancelOrder/" + id;
            _ = CallApiUpdateOrderAsync(BaseUrl);
            return RedirectToAction("DetailOrder", new { id = id });
        }
        public IActionResult ConfirmOrder(int id)
        {
            string BaseUrl = "http://localhost:3000/ConfirmOrder/" + id;
            _ = CallApiUpdateOrderAsync(BaseUrl);
            return RedirectToAction("DetailOrder", new { id = id });
        }
        public IActionResult CompleteOrder(int id)
        {
            string BaseUrl = "http://localhost:3000/CompleteOrder/" + id;
            _ = CallApiUpdateOrderAsync(BaseUrl);
            return RedirectToAction("DetailOrder", new { id = id });
        }
        public async Task GetUsersAsync()
        {
            string BaseUrl = "http://localhost:3000/users";
            //Users = JsonConvert.DeserializeObject<List<User>>(getdata(BaseUrl));
            Task<string> datatask = CallApiGetData(BaseUrl);
            Users = JsonConvert.DeserializeObject<List<User>>(await datatask);
        }
        public async Task GetOrdersAsync()
        {
            string BaseUrl = "http://localhost:3000/orders/";
            //Orders = JsonConvert.DeserializeObject<List<Order>>(getdata(BaseUrl));
            Task<string> datatask = CallApiGetData(BaseUrl);
            Orders = JsonConvert.DeserializeObject<List<Order>>(await datatask);
        }
        public async Task GetOrderAsync(int orderId)
        {
            string BaseUrl = "http://localhost:3000/order/" + orderId;
            //Order = JsonConvert.DeserializeObject<List<Order>>(getdata(BaseUrl))[0];
            Task<string> datatask = CallApiGetData(BaseUrl);
            Order = JsonConvert.DeserializeObject<List<Order>>(await datatask)[0];
        }
        public async Task GetOrdersAsync(string userId)
        {
            string BaseUrl = "http://localhost:3000/orders/users/" + userId;
            //Orders = JsonConvert.DeserializeObject<List<Order>>(getdata(BaseUrl));
            Task<string> datatask = CallApiGetData(BaseUrl);
            Orders = JsonConvert.DeserializeObject<List<Order>>(await datatask);
        }
        public async Task GetDetailOrdersAsync(int orderId)
        {
            string BaseUrl = "http://localhost:3000/OrderDetails/" + orderId;
            //DetailOrders = JsonConvert.DeserializeObject<List<DetailOrder>>(getdata(BaseUrl));
            Task<string> datatask = CallApiGetData(BaseUrl);
            DetailOrders = JsonConvert.DeserializeObject<List<DetailOrder>>(await datatask);
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
    }
}
