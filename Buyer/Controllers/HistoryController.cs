using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Buyer.Mvc.Models;
using Buyer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

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
        public void viewLists()
        {
            ListOrderHeader = Context.OrderHeaderDbs.FromSqlRaw("dbo.[usp_Order] @p0, @p1,@p2", "ViewOrderHeader", 0, CustID).ToList();
        }
        public IActionResult Index()
        {
            UserID = HttpContext.Session.GetString("UserID").ToString();
            CustID = HttpContext.Session.GetString("CustID").ToString();
            viewLists();
            var model = (ListOrderHeader);
            return View(model);
        }
        public IActionResult DetailOrder(int id)
        {
            if(id > 0)
            {
                ListOrderLine = Context.OrderLineDbs.FromSqlRaw("dbo.[usp_Order] @p0, @p1,@p2", "ViewOrderLine", id, CustID).ToList();
                OrderHeader = Context.OrderHeaderDbs.FromSqlRaw("dbo.[usp_Order] @p0, @p1", "ViewOneOrderHeader", id).ToList()[0];
            }
                
            var model = (OrderHeader,ListOrderLine);
            return View(model);
            //return RedirectToAction("Index", new { OrderID = int.Parse(OrderID)});
        }

        public IActionResult CancelOrder(int id)
        {
            Context.Database.ExecuteSqlRaw("dbo.[usp_Order] @p0, @p1", "CancelOrder", id);
            return RedirectToAction("DetailOrder", new { id = id });
        }
    }
}
