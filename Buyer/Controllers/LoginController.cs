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
using System.ComponentModel.DataAnnotations;

namespace Buyer.Mvc.Controllers
{
	public class LoginController : Controller
	{
		[BindProperty]
		public string UserID { get; set; } = "";
		[BindProperty]
		public string ErrUserID { get; set; } = "";
		[BindProperty]
		public string Password { get; set; } = "";
		[BindProperty]
		public string ErrPassword { get; set; } = "";
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
			var model = (ErrUserID, ErrPassword);
			return View(model);
		}

		[HttpPost]
		public IActionResult LoginToPage(string UserID, string Password)
		{
			foreach (BuyerUser buyerUser in ListBuyerUser)
			{
				if (buyerUser.BuyerId.Trim().Equals(UserID.Trim()))
				{
					if (buyerUser.Pass.Trim().Equals(Password.Trim()))
					{
						HttpContext.Session.SetString("usercode", buyerUser.BuyerId.Trim());
						HttpContext.Session.SetString("username", buyerUser.BuyerName.Trim());
						return LocalRedirect("~/Home/Index");
					}
					ErrPassword = "Incorreact password!";
					return RedirectToAction("Index");
				}
			}
			ErrUserID = "User is not exist!";
			return RedirectToAction("Index");
		}
	}
}
