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
		public string userbuyer { get; set; }
		public List<ItemSample> ListRegisteredItem { get; set; }
		public List<ItemRegistration> ListItemRegistration { get; set; }
		public List<ItemOrder> ListItemOrder { get; set; }

		public List<ItemRegistration> tempListItemRegistration = new List<ItemRegistration>() ;
		public List<string> tempListItemCode = new List<string>();
		public List<string> tempListColor = new List<string>();
		public List<string> tempListSize= new List<string>();

		//filter Item
		public List<ItemCategory> listCategory = new List<ItemCategory>();
		public List<ItemSeason> listSeason = new List<ItemSeason>();

		private DatabaseContext Context { get; }
		public HomeController(DatabaseContext _db)
		{
			//TempData["username"] = HttpContext.Session.GetString("username");
			Context = _db;
			ListItemRegistration = Context.ItemRegistrationDbs.FromSqlRaw("dbo.item_registration @p0", "ViewItemRegistration").ToList();
			ListRegisteredItem = Context.ItemSampleDbs.FromSqlRaw("dbo.sp_item @p0", "viewRegisteredItem").ToList();
			listSeason = Context.ItemSeasonDbs.FromSqlRaw("dbo.[sp_item] @p0", "ViewSeason").ToList();
			listCategory = Context.ItemCategoryDbs.FromSqlRaw("dbo.[sp_item] @p0", "ViewCategory").ToList();
			ListItemOrder = convertToListItemOrder();
			//userbuyer = HttpContext.Session.GetString("usercode");
		}
		public List<string> getListSize(string ItemCode, string ColorName)
		{
			tempListSize = new List<string>();

			foreach (ItemRegistration ItemReg in ListItemRegistration)
			{
				if (ItemReg.ItemCode.Trim().Equals(ItemCode) && ItemReg.ColorName.Trim().Equals(ColorName))
				{
					tempListSize.Add(ItemReg.SizeName.Trim());
				}
			}

			return tempListSize;
		}
		public List<string> getListColor(string ItemCode)
		{
			tempListColor = new List<string>();
			tempListItemRegistration = new List<ItemRegistration>();
			foreach (ItemRegistration ItemReg in ListItemRegistration)
			{
				if (ItemReg.ItemCode.Trim().Equals(ItemCode))
				{
					tempListItemRegistration.Add(ItemReg);
				}
			}
			tempListColor.Add(tempListItemRegistration[0].ColorName.Trim());
			for(int i = 1; i < tempListItemRegistration.Count; i++)
			{
				if (!tempListColor.Contains(tempListItemRegistration[i].ColorName.Trim()))
				{
					tempListColor.Add(tempListItemRegistration[i].ColorName.Trim());
				}
			}

			return tempListColor;
		}
		public List<ItemOrder> convertToListItemOrder()
		{
			ListItemOrder = new List<ItemOrder>();
			foreach (ItemSample ItemSample in ListRegisteredItem)
			{
				ItemOrder tempItemOrder = new ItemOrder();
				tempItemOrder.ItemCode = ItemSample.ItemCode;
				tempItemOrder.Description = ItemSample.Description;
				tempItemOrder.CategoryCode = ItemSample.CategoryCode;
				tempItemOrder.FitCode = ItemSample.FitCode;
				tempItemOrder.FobPrice = ItemSample.FobPrice;
				tempItemOrder.SeasonCode = ItemSample.SeasonCode;
				tempItemOrder.ProtoSampleCode = ItemSample.ProtoSampleCode;
				tempItemOrder.Picture = ItemSample.Picture;

				List<ItemOrderColor> tempListItemOrderColor = new List<ItemOrderColor>();

				List<string> tempListColorOfItemCode = getListColor(ItemSample.ItemCode);

				foreach(string color in tempListColorOfItemCode)
				{ 
					ItemOrderColor tempItemOrderColor = new ItemOrderColor();
					tempItemOrderColor.ColorName = color;

					tempItemOrderColor.ListSizeName = getListSize(ItemSample.ItemCode, color);

					tempListItemOrderColor.Add(tempItemOrderColor);
				}
				tempItemOrder.ListItemOrderColor = tempListItemOrderColor;
				if (tempItemOrder!=null)
					ListItemOrder.Add(tempItemOrder);
			}

			return ListItemOrder;
		}
		public IActionResult Index()
		{
			userbuyer = HttpContext.Session.GetString("UserID").ToString();
			TempData["UserID"] = userbuyer;
			var model = (ListItemOrder, listSeason, listCategory);
			return View(model);
		}

		public IActionResult InsertItemOrder(string itemCode, string tempStringOrder , string retailPrice)
		{
			ItemSample itemSample = getItemSample(itemCode.Trim());
			int OrderId = getOrderId();

			string[] tempString = tempStringOrder.Trim().Split("  ");
			List<ItemOrderBuyer> listItemOrderBuyer = new List<ItemOrderBuyer>();
			int dem = tempString.Length;
			for (int i = 0; i < dem-2; i=i+3)
			{
				ItemOrderBuyer itemOrderBuyer = new ItemOrderBuyer();
				itemOrderBuyer.ItemCode = itemCode.Trim();
				itemOrderBuyer.Color = tempString[i];
				itemOrderBuyer.Size = tempString[i+1];
				itemOrderBuyer.Quantity = int.Parse(tempString[i+2]);
				listItemOrderBuyer.Add(itemOrderBuyer);
			}

			foreach(ItemOrderBuyer temp in listItemOrderBuyer)
			{
				if(temp.Quantity > 0)
				{
					Context.Database.ExecuteSqlRaw("dbo.[ItemOrder] @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7", "InsertLineOrder", OrderId, itemCode.Trim(), temp.Color + '#' + temp.Size, temp.Color, temp.Size, temp.Quantity, int.Parse(retailPrice));
				}
			}

			Context.Database.ExecuteSqlRaw("dbo.[ItemOrder] @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8", "InsertOrderHeader", OrderId, "", "", "", "", 0, 0, userbuyer);
			return RedirectToAction("Index");
		}

		public ItemSample getItemSample(string itemCode)
		{
			ItemSample tempItemSample = Context.ItemSampleDbs.FromSqlRaw("dbo.[sp_item] @p0,@p1", "ViewOneItem", itemCode).ToList()[0];
			return tempItemSample;
		}

		public int getOrderId()
		{
			List<OrderHeader> listOrderHeader = Context.OrderHeaderDbs.FromSqlRaw("dbo.[ItemOrder] @p0", "ViewOrderHeader").ToList();
			int maxOrderId = 0;
			foreach(OrderHeader orderHeader in listOrderHeader)
			{
				if (orderHeader.OrderID > maxOrderId)
					maxOrderId = orderHeader.OrderID;
			}
			return maxOrderId + 1;
		}
	}
}
