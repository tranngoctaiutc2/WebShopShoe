using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShoeShop.Models;
using WebShoeShop.Models.EF;

namespace WebShoeShop.Areas.Admin.Controllers
{
    public class StatisticalController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Statistical
        public ActionResult Index()
        {
          
            return View();
          
        }
        public ActionResult GetCustomerOrder(string Searchtext)
        {
            var items = db.Database.SqlQuery<CustomerOrderViewModel>("EXEC sp_GetCustomerOrderStatistics").Take(10).ToList();
            if (!string.IsNullOrEmpty(Searchtext))
            {
                string searchTextLower = Searchtext.ToLowerInvariant();
                items = items.Where(x => x.CustomerName.IndexOf(searchTextLower, StringComparison.OrdinalIgnoreCase) != -1).ToList();
            }
            return View(items);
        }
        public ActionResult GetProductBestSeller(string Searchtext)
        {
            var items = db.Database.SqlQuery<ProductViewModel>("EXEC sp_GetProductBestSeller").Take(7).ToList();
            if (!string.IsNullOrEmpty(Searchtext))
            {
                string searchTextLower = Searchtext.ToLowerInvariant();
                items = items.Where(x =>x.ProductName.IndexOf(searchTextLower, StringComparison.OrdinalIgnoreCase) != -1).ToList();
            }
            return View(items);
        }
        public ActionResult GetOrders()
        {
            List<Order> orders = GetOrdersFromDatabase(); // Hàm lấy danh sách đơn hàng từ cơ sở dữ liệu

            List<Order> ordersInDay = orders.Where(o => o.CreatedDate.Date == DateTime.Today.Date).ToList();
            List<Order> ordersInMonth = orders.Where(o => o.CreatedDate.Month == DateTime.Today.Month && o.CreatedDate.Year == DateTime.Today.Year).ToList();

            OrderViewModel viewModel = new OrderViewModel
            {
                OrdersInDay = ordersInDay,
                OrdersInMonth = ordersInMonth
            };

            return View(viewModel);
        }
        public List<Order> GetOrdersFromDatabase()
        {
            List<Order> orders = db.Orders.ToList();
            return orders;
        }
        [HttpGet]
        public ActionResult GetStatistical(string fromDate, string toDate)
        {
            var query = from o in db.Orders
                        join od in db.OrderDetails
                        on o.Id equals od.OrderId
                        join p in db.Products
                        on od.ProductId equals p.Id
                        select new
                        {
                            CreatedDate = o.CreatedDate,
                            Quantity = od.Quantity,
                            Price = od.Price,
                            OriginalPrice = p.OriginalPrice2
                        };
            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime startDate = DateTime.ParseExact(fromDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.CreatedDate >= startDate);
            }
            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime endDate = DateTime.ParseExact(toDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.CreatedDate < endDate);
            }

            var result = query.GroupBy(x => DbFunctions.TruncateTime(x.CreatedDate)).Select(x => new
            {
                Date = x.Key.Value,
                TotalBuy = x.Sum(y => y.Quantity * y.OriginalPrice),
                TotalSell = x.Sum(y => y.Quantity * y.Price),
            }).Select(x => new
            {
                Date = x.Date,
                DoanhThu = x.TotalSell,
                LoiNhuan = x.TotalSell - x.TotalBuy
            });
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

    }
}