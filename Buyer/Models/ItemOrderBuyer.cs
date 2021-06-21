using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buyer.Mvc.Models
{
	public class ItemOrderBuyer
	{
		public string ItemCode { get; set; }
		public string Color { get; set; }
		public string Size { get; set; }
		public int Quantity { get; set; }
	}
}
