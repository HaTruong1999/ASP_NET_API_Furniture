using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buyer.Mvc.Models
{
    public class OrderLine
    {
        public string ItemCode { get; set; }
        public string URLPicture { get; set; }
        public string ColorCode { get; set; }
        public string ColorName { get; set; }
        public string URLColor { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public decimal FOBPrice { get; set; }
        public string Currency { get; set; }
        public string RetailPrice { get; set; }
        public string CurrencyRetail { get; set; }
    }
}
