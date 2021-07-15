using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buyer.Mvc.Models.Order
{
	public class Order
	{
		public int id { get; set; }
		public string userId { get; set; }
		public bool bought { get; set; }
		public string date { get; set; }
		public string status { get; set; }
		public Order(int id, string userId)
		{
			this.id = id;
			this.userId = userId;
			this.bought = false;
			this.date = DateTime.UtcNow.Date.ToString("dd/MM/yyyy");
			this.status = "Shopping";
		}

		public Order()
		{
		}

	}
}
