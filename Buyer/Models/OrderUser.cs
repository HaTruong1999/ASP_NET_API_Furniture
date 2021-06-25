using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buyer.Mvc.Models
{
	public class OrderUser
	{
		public string UserID { get; set; }
		public string Password { get; set; }
		public string CustID { get; set; }
		public bool isLogin { get; set; }
	}
}
