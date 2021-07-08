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
using System.Security.Cryptography;
using System.Text;

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
		[BindProperty(SupportsGet = true)]
		public string UserIDLogOut { get; set; } = "";
		public List<OrderUser> ListOrderUser { get; set; }
		private DatabaseContext Context { get; }
		public LoginController(DatabaseContext _db)
		{
			Context = _db;
			ListOrderUser = Context.OrderUserDbs.FromSqlRaw("dbo.[usp_UserLogin] @p0", "ViewListUser").ToList();

		}
		public IActionResult Index(string UserID, string ErrUserID, string Password, string ErrPassword)
		{
            if (UserIDLogOut != "")
            {
				HttpContext.Session.Remove("UserID");
				//Context.Database.ExecuteSqlRaw("dbo.[usp_UserLogin] @p0,@p1", "UserIsLogout", UserIDLogOut);
			}
			var model = (UserID,ErrUserID,Password, ErrPassword);
			return View(model);
		}

		[HttpPost]
		public IActionResult LoginToPage(string UserID, string Password)
		{
			Password = MD5Hash(Password);
			foreach (OrderUser orderUser in ListOrderUser)
			{
				if (orderUser.UserID.Trim().Equals(UserID.Trim()))
				{
                    if (orderUser.isLogin)
                    {
						return RedirectToAction("Index", new { UserID = UserID, ErrUserID = "User is logged on other device!", Password = Password, ErrPassword = "" });
					}
					if (orderUser.Password.Trim().Equals(Password.Trim()))
					{
						HttpContext.Session.SetString("UserID", orderUser.UserID.Trim().ToString());
						HttpContext.Session.SetString("CustID", orderUser.CustID.Trim().ToString());
						//Context.Database.ExecuteSqlRaw("dbo.[usp_UserLogin] @p0,@p1", "UserIsLogin", orderUser.UserID.Trim());
						return LocalRedirect("~/Home/Index");
					}
					return RedirectToAction("Index", new { UserID = UserID, ErrUserID = "", Password = Password, ErrPassword = "Incorreact password!" });
				}
			}
			return RedirectToAction("Index", new { UserID = UserID, ErrUserID = "User is not exist!", Password = Password,  ErrPassword = "" }) ;
		}

		public IActionResult LogOut(string UserID)
		{
			Context.Database.ExecuteSqlRaw("dbo.[usp_UserLogin] @p0,@p1", "UserIsLogout", UserID);
			HttpContext.Session.Remove("usercode");
			return RedirectToAction("Index", new { UserID = UserID, ErrUserID = "", Password = Password, ErrPassword = "" });
		}
		public string MD5Hash(string text)
		{
			MD5 md5 = new MD5CryptoServiceProvider();

			//compute hash from the bytes of text  
			md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

			//get hash result after compute it  
			byte[] result = md5.Hash;

			StringBuilder strBuilder = new StringBuilder();
			for (int i = 0; i < result.Length; i++)
			{
				//change it into 2 hexadecimal digits  
				//for each byte  
				strBuilder.Append(result[i].ToString("x2"));
			}

			return strBuilder.ToString();
		}
	}
}
