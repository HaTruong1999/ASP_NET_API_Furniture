﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buyer.Mvc.Models
{
	public class OrderHeader
	{
		public int OrderID { get; set; }
		public DateTime OrderDate { get; set; }
		public string BuyerID { get; set; }
		public string Status { get; set; }
	}
}
