using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebShoeShop.Models.EF;

namespace WebShoeShop.Models
{
    public class OrderViewModel
    {
        [Required(ErrorMessage = "Tên khách hàng không để trống")]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "Số điện thoại không để trống")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Địa chỉ khổng để trống")]
        public string Address { get; set; }
        public string Email { get; set; }
        public string CustomerId { get; set; }
        public int TypePayment { get; set; }
        public int TypeShip { get; set; }
        public int TypePaymentVN { get; set; }
        public DateTime CreateDate { get; set; }
        public List<EF.Order> OrdersInDay { get; set; }
        public List<EF.Order> OrdersInMonth { get; set; }
        public List<ExcelModel> GetOrderDetailForExcel(int orderId)
    {
        using(var db = new ApplicationDbContext())
        {
                var lsorder = db.Orders.FirstOrDefault(x => x.Id == orderId);
                var lsDetail = db.OrderDetails.Join(db.Products, x => x.ProductId, y=>y.Id,(x,y)=>new {
            detail = x,product=y 
            }
            ).Where(x => x.detail.OrderId == orderId).Select(x=> new ExcelModel
            {
               OrderId= x.detail.OrderId,
              ProductName=  x.product.Title,
              Quantity=  x.detail.Quantity,
              Price=  x.detail.Price,
               
            }).ToList();
                foreach (var detail in lsDetail)
                {
                    detail.CustomerName = lsorder.CustomerName;
                    detail.Address= lsorder.Address;
                    detail.Phone = lsorder.Phone;
                    detail.CreateDate = lsorder.CreatedDate;
                }
                return lsDetail;
        }
    }
    }
    
}