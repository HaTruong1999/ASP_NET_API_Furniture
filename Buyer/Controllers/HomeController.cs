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
		[BindProperty]
		public bool notifiOrderSuccess { get; set; } = false;
		public List<ItemByCustomer> ListItemByCoustomer = new List<ItemByCustomer>();
		public List<ItemByCustomerOrder> ListItemByCoustomerOrder = new List<ItemByCustomerOrder>();
		public List<CartDetail> ListCartDetail = new List<CartDetail>();
		//filter Item
		public List<Division> listDivision = new List<Division>();
		public List<ItemSeason> listSeason = new List<ItemSeason>();
		public List<ItemCollection> listCollection = new List<ItemCollection>();
		public List<PriceCurrency> listCurrency = new List<PriceCurrency>();

		public int Division { get; set; } = 0;
		public string Gender { get; set; } = "%";
		public string Season { get; set; } = "%";
		public string Collection { get; set; } = "%";
		private DatabaseContext Context { get; }
		public HomeController(DatabaseContext _db)
		{
			Context = _db;
		}

		public void viewLists(string gender, string season, string collection , int division)
		{
			listDivision = Context.DivisionDbs.FromSqlRaw("dbo.[usp_Item] @p0", "ViewDivision").ToList();
			if(division==0)
				division = listDivision[0].DivisionCode;
			listSeason = Context.ItemSeasonDbs.FromSqlRaw("dbo.[usp_Item] @p0", "ViewSeason").ToList();
			if(!season.Equals("%"))
				listCollection = Context.ItemCollectionDbs.FromSqlRaw("dbo.[usp_Item] @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7", "ViewCollection", "", "", "", "", "", "", season.Trim()).ToList();
			else
				listCollection = Context.ItemCollectionDbs.FromSqlRaw("dbo.[usp_Item] @p0", "ViewAllCollection").ToList();
			listCurrency = Context.PriceCurrencyDbs.FromSqlRaw("dbo.[usp_Currency] @p0", "ViewCurrency").ToList();
			ListCartDetail = Context.CartDetailDbs.FromSqlRaw("dbo.[usp_Order_Cart] @p0, @p1, @p2", "ViewTotalCart" ,"", CustID).ToList();
			ListItemByCoustomer = Context.ItemByCustomerDbs.FromSqlRaw("dbo.[usp_InfoItemByCust] @p0, @p1,@p2,@p3,@p4", CustID, gender, season, collection, division).ToList();
		}

		//get color and size for each item
		public void getListItemByCustomerToOrder()
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
		public IActionResult Index(bool notifiOrderSuccess, string Gender, string Season, string Collection, int Division)
		{
			if (string.IsNullOrEmpty(Gender))
				Gender = "%";
			if (string.IsNullOrEmpty(Season))
				Season = "%";
			if (string.IsNullOrEmpty(Collection))
				Collection = "%";
			UserID = HttpContext.Session.GetString("UserID").ToString();
			CustID = HttpContext.Session.GetString("CustID").ToString();
			viewLists(Gender, Season, Collection, Division);
			getListItemByCustomerToOrder();
			var model = (ListItemByCoustomerOrder, ListCartDetail, listSeason, listCurrency, listCollection , listDivision, notifiOrderSuccess, Gender , Season, Collection , Division);
			return View(model);
		}

		[HttpPost]
		public IActionResult InsertItemOrderToCart(string Division, string Gender, string Season, string Collection,string ItemCode, int FOBPrice, string Currency, string RetailPrice, string RetailCurrency, string arrayVariant)
		{
			UserID = HttpContext.Session.GetString("UserID").ToString();
			CustID = HttpContext.Session.GetString("CustID").ToString();

			string[] listVariant = arrayVariant.Split(',');
			int dem = listVariant.Length;

			string ColorCode ="";
			string SizeCode = "";
			string Variant = "";
			int Quantity = 0;
			for (int i = 0; i < dem - 3; i = i + 3)
            {
				ColorCode = listVariant[i];
				SizeCode = listVariant[i + 1];
				Variant = ColorCode + '#' + SizeCode;
				Quantity = int.Parse(listVariant[i + 2]);

				Context.Database.ExecuteSqlRaw("[dbo].[usp_Order_Cart] @p0, @p1, @p2, @p3, @p4 , @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12, @p13", "InsertOrderCart", UserID, CustID, 1, ItemCode.Trim(), Variant, ColorCode, SizeCode, Quantity, FOBPrice, Currency, 0, RetailPrice, RetailCurrency);
			}
			return RedirectToAction("Index", new { notifiOrderSuccess = false, Gender = Gender, Season = Season, Collection = Collection, Division = int.Parse(Division) });
		}
		[HttpPost]
		public IActionResult InsertOrder(string Gender, string Season, string Collection, string Division)
		{
			UserID = HttpContext.Session.GetString("UserID").ToString();
			CustID = HttpContext.Session.GetString("CustID").ToString();
			Context.Database.ExecuteSqlRaw("[dbo].[usp_Order_Cart] @p0, @p1, @p2", "ConfirmOrderCart", UserID, CustID);
			return RedirectToAction("Index", new { notifiOrderSuccess = true, Gender = Gender, Season = Season, Collection = Collection, Division = int.Parse(Division) });
		}

		[HttpPost]
		public IActionResult FilterItem(string Gender,string Season, string Collection, string Division)
		{
			return RedirectToAction("Index", new { notifiOrderSuccess = false, Gender = Gender , Season = Season, Collection = Collection , Division = int.Parse(Division) });
		}
	}
}
