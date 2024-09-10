using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShoeShop.Models
{
    public class ProfitViewModel
    {
        public DateTime CreatedDate { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal OriginalPrice { get; set; }
    }
}