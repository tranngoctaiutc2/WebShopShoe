using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShoeShop.Models
{
    public class CustomerOrderViewModel
    {
        public string CustomerName { get; set; }
        public int OrderCount { get; set; }
        public string Email {  get; set; }
        public string Phone { get; set; }
    }
}