using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buyer.Mvc.Models
{
    public class CartDetail
    {
        public string ItemCode { get; set; }
        public string Collection { get; set; }
        public string URLPicture { get; set; }
        public decimal FOBPrice { get; set; }
        public string RetailPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
