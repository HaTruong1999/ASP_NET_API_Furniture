using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buyer.Mvc.Models
{
	public class ItemRegistration
	{
		public int ID { get; set; }
		public string ItemCode { get; set; }
		public string VariantCode { get; set; }
		public string ColorName { get; set; }
		public string SizeName { get; set; }
	}
}
