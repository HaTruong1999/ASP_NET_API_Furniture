using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buyer.Mvc.Models.DetailOrder
{
	public class DetailOrder
	{
		public int id { get; set; }
		public int orderId { get; set; }
		public string productId { get; set; }
		public int quantity { get; set; }
		public DetailOrder(int id, int orderId, string productId, int quantity)
		{
			this.id = id;
			this.orderId = orderId;
			this.productId = productId;
			this.quantity = quantity;
		}
	}
}
