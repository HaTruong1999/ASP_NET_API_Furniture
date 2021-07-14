using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buyer.Mvc.Models.Product
{
	public class Product
	{
		public Product(string id, string name, float price, float oldPrice, string photo, int amount, string roomId, string producerId)
		{
			this.id = id;
			this.name = name;
			this.price = price;
			this.oldPrice = oldPrice;
			this.photo = photo;
			this.amount = amount;
			this.roomId = roomId;
			this.producerId = producerId;
		}

		public string id { get; set; }
        public string name { get; set; }
        public float price { get; set; }
        public float oldPrice { get; set; }
        public string photo { get; set; }
        public int amount { get; set; }
        public string roomId { get; set; }
        public string producerId { get; set; }
    }
}
