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
	public class HomeController : Controller
	{
		public string UserID { get; set; }
		public string CustID { get; set; }

		public List<ItemByCustomer> ListItemByCoustomer = new List<ItemByCustomer>();
		public List<ItemByCustomerOrder> ListItemByCoustomerOrder = new List<ItemByCustomerOrder>();
		//filter Item
		public List<ItemCategory> listCategory = new List<ItemCategory>();
		public List<ItemSeason> listSeason = new List<ItemSeason>();
		private DatabaseContext Context { get; }
		public HomeController(DatabaseContext _db)
		{
			Context = _db;
		}

		public void viewLists()
		{
			listSeason = Context.ItemSeasonDbs.FromSqlRaw("dbo.[sp_item] @p0", "ViewSeason").ToList();
			listCategory = Context.ItemCategoryDbs.FromSqlRaw("dbo.[sp_item] @p0", "ViewCategory").ToList();
			ListItemByCoustomer = Context.ItemByCustomerDbs.FromSqlRaw("dbo.[usp_InfoItemByCust] @p0", CustID).ToList();
		}

		//get color and size for each item
		public void getListItemByCostomerToOrder()
		{
			foreach (ItemByCustomer itemByCustomer in ListItemByCoustomer)
			{
				ItemByCustomerOrder itemByCustomerOrder = new ItemByCustomerOrder();
				itemByCustomerOrder.ItemByCustomer = itemByCustomer;
				itemByCustomerOrder.ListColor = Context.ItemColorDbs.FromSqlRaw("dbo.[usp_Variant_Item] @p0, @p1", "ViewColorByItem" , itemByCustomer.ItemCode.Trim()).ToList();
				itemByCustomerOrder.ListSize = Context.ItemSizeDbs.FromSqlRaw("dbo.[usp_Variant_Item] @p0, @p1", "ViewSizeByItem", itemByCustomer.ItemCode.Trim()).ToList();
				itemByCustomerOrder.ListColorAndSizeByItem = Context.ColorAndSizeByItemDbs.FromSqlRaw("dbo.[usp_Order_Cart] @p0, @p1, @p2, @p3, @p4", "QuatityByItemSizeColor", "" , CustID , 1 , itemByCustomer.ItemCode.Trim()).ToList();

				ListItemByCoustomerOrder.Add(itemByCustomerOrder);
			}
		}
		public IActionResult Index()
		{
			UserID = HttpContext.Session.GetString("UserID").ToString();
			CustID = HttpContext.Session.GetString("CustID").ToString();
			viewLists();
			getListItemByCostomerToOrder();
			var model = (ListItemByCoustomerOrder, listSeason, listCategory);
			return View(model);
			
			//if (UserID != null)
			//{
			//	viewLists();
			//	getListItemByCostomerToOrder();
			//	var model = (ListItemByCoustomerOrder, listSeason, listCategory);
			//	return View(model);
			//}else
			//	return LocalRedirect("~/Login/Index");

		}
	}
}
