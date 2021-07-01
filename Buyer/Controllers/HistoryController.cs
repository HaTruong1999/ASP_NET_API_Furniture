using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Buyer.Mvc.Models;
using Buyer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;

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
        public List<OrderLine> ListOrderLine = new List<OrderLine>();
        public void viewLists(int OrderID)
        {
            if(OrderID > 0)
                ListOrderLine = Context.OrderLineDbs.FromSqlRaw("dbo.[usp_Order] @p0, @p1,@p2", "ViewOrderLine", OrderID, CustID).ToList();
            ListOrderHeader = Context.OrderHeaderDbs.FromSqlRaw("dbo.[usp_Order] @p0, @p1,@p2", "ViewOrderHeader", 0, CustID).ToList();
        }
        public IActionResult Index(int OrderID)
        {
            UserID = HttpContext.Session.GetString("UserID").ToString();
            CustID = HttpContext.Session.GetString("CustID").ToString();
            viewLists(OrderID);
            var model = (ListOrderHeader, ListOrderLine, OrderID);
            return View(model);
        }
        [HttpPost]
        public IActionResult ViewDetailOrder(string OrderID)
        {

            return RedirectToAction("Index", new { OrderID = int.Parse(OrderID)});
        }
        [HttpPost]
        public IActionResult CancleOrder(string OrderIDCancle)
        {
            Context.Database.ExecuteSqlRaw("dbo.[usp_Order] @p0, @p1,@p2", "CancelOrder", int.Parse(OrderIDCancle), "");
            return RedirectToAction("Index", new { OrderID = 0 });
        }
    }
}
