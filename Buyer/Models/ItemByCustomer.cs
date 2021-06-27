using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buyer.Mvc.Models
{
	public class ItemByCustomer
	{
		public string ItemCode { get; set; }
		public string Description { get; set; }
		public string CategoryCode { get; set; }
		public string CategoryName { get; set; }
		public string SeasonCode { get; set; }
		public string SeasonName { get; set; }
		public string CollectionCode { get; set; }
		public string CollectionName { get; set; }
		public string FormCode { get; set; }
		public string FormName { get; set; }
		public string Fabric { get; set; }
		public decimal FOBPrice { get; set; }
		public string CurrencyCode { get; set; }
		public string URLPicture { get; set; }
		
	}
}
