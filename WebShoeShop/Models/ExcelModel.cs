using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShoeShop.Models
{
    public class ExcelModel
    {
        public string CustomerName { get; set; }
        public string Address {  get; set; }
        public string Phone {  get; set; }
        public DateTime CreateDate { get; set; }
        public int OrderId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int Size { get; set; }
    }
}