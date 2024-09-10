using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShoeShop.Models
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public int TotalQuantity { get; set; }
        public string Alias { get; set; }

    }
}