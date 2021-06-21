using Buyer.Data;
using Buyer.Mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buyer.Mvc.Controllers
{
	public class LoginController : Controller
	{
		public bool enableErrBuyerUser { get; set; } = false;
		public bool enableErrPassword { get; set; } = false;
		public List<BuyerUser> ListBuyerUser { get; set; }
		private DatabaseContext Context { get; }
		public LoginController(DatabaseContext _db)
		{
			Context = _db;
			ListBuyerUser = Context.BuyerUserDbs.FromSqlRaw("dbo.[UserLogin] @p0", "ViewUser").ToList();
		}
		public IActionResult Index()
		{
			var model = (enableErrBuyerUser, enableErrPassword);
			return View(model);
		}

		[HttpPost]
		public IActionResult LoginToPage(string BuyerUser, string Pass)
		{
			foreach (BuyerUser buyerUser in ListBuyerUser)
			{
				if (buyerUser.BuyerId.Trim().Equals(BuyerUser.Trim()))
				{
					if (buyerUser.Pass.Trim().Equals(Pass.Trim()))
					{
						HttpContext.Session.SetString("usercode", buyerUser.BuyerId.Trim());
						HttpContext.Session.SetString("username", buyerUser.BuyerName.Trim());
						return LocalRedirect("~/Home/Index");
					}
					enableErrPassword = true;
					return RedirectToAction("Index");
				}
			}
			enableErrBuyerUser = true;
			return RedirectToAction("Index");
		}
	}
}
