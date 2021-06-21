using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buyer.Mvc.Models
{
	public class ItemSample
	{
		public string ItemCode { get; set; }
		public string Description { get; set; }
		public string CategoryCode { get; set; }
		public string SeasonCode { get; set; }
		public string FitCode { get; set; }
		public string ProtoSampleCode { get; set; }
		public decimal FobPrice { get; set; }
		public string Picture { get; set; }
	}
}
