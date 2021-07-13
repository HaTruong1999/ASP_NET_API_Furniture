using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buyer.Mvc.Models.User
{
	public class User
	{
        public string username { get; set; }
        public string password { get; set; }
        public string fullname { get; set; }
        public string photo { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
    }
}
