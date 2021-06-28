using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buyer.Mvc.Models
{
	public class ItemByCustomerOrder
	{
		public ItemByCustomer ItemByCustomer { get; set; }
		public List<ItemSize> ListSize { get; set; }
		public List<ItemColor> ListColor { get; set; }
		public List<ColorAndSizeByItem> ListColorAndSizeByItem { get; set; }

		public int GetQuatityByVariant(string colorCode , string sizeCode )
        {
			foreach (ColorAndSizeByItem c in this.ListColorAndSizeByItem)
				if (c.ColorCode == colorCode && c.SizeCode == sizeCode)
					return c.Quantity;
			return 0;
        }

	}
}
