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
		public List<OrderUser> ListOrderUser { get; set; }
		private DatabaseContext Context { get; }
		public LoginController(DatabaseContext _db)
		{
			Context = _db;
			ListOrderUser = Context.OrderUserDbs.FromSqlRaw("dbo.[usp_UserLogin] @p0", "ViewListUser").ToList();
		}
		public IActionResult Index(string UserID, string ErrUserID, string Password, string ErrPassword)
		{
			var model = (UserID,ErrUserID,Password, ErrPassword);
			return View(model);
		}

		[HttpPost]
		public IActionResult LoginToPage(string UserID, string Password)
		{
			foreach (OrderUser orderUser in ListOrderUser)
			{
				if (orderUser.UserID.Trim().Equals(UserID.Trim()))
				{
					if (orderUser.Password.Trim().Equals(Password.Trim()))
					{
						HttpContext.Session.SetString("UserID", orderUser.UserID.Trim());
						return LocalRedirect("~/Home/Index");
					}
					return RedirectToAction("Index", new { UserID = UserID, ErrUserID = "", Password = Password, ErrPassword = "Incorreact password!" });
				}
			}
			return RedirectToAction("Index", new { UserID = UserID, ErrUserID = "User is not exist!", Password = Password,  ErrPassword = "" }) ;
		}
	}
}
