using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShoeShop.Models;
using PagedList;
using WebShoeShop.Models.ViewModels;
using WebShoeShop.Models.EF;
using ClosedXML.Excel;

namespace WebShoeShop.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        public OrderViewModel _orderViewModel= new OrderViewModel();
        // GET: Admin/Order
        public ActionResult Index(string Searchtext, int? page, int? month, int? status)
        {
            var pageSize = 10;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<Order> items = db.Orders.OrderByDescending(x => x.Id);

            if (month.HasValue)
            {
                items = items.Where(x => x.CreatedDate.Month == month);
            }
            if (status.HasValue)
            {
                items = items.Where(x => x.Status == status);
            }
            if (!string.IsNullOrEmpty(Searchtext))
            {
                string searchTextLower = Searchtext.ToLowerInvariant();
                items = items.Where(x => x.Code.IndexOf(searchTextLower, StringComparison.OrdinalIgnoreCase) != -1
                    || x.CustomerName.IndexOf(searchTextLower, StringComparison.OrdinalIgnoreCase) != -1
                    || x.Phone.IndexOf(searchTextLower, StringComparison.OrdinalIgnoreCase) != -1
                );
            }

            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;
            ViewBag.Month = month;

            return View(items);
        }



        public ActionResult View(int id)
        {
            var item = db.Orders.Find(id);
            return View(item);
        }

        public ActionResult Partial_SanPham(int id)
        {
            var items = db.OrderDetails.Where(x => x.OrderId == id).ToList();
            return PartialView(items);
        }

        [HttpPost]
        public ActionResult UpdateTT(int id, int trangthai, int thanhtoan)
        {
            var item = db.Orders.Find(id);
            if (item != null)
            {
                db.Orders.Attach(item);
                item.Status = trangthai;
                item.StatusPayMent = thanhtoan;
                db.Entry(item).Property(x => x.TypePayment).IsModified = true;
                db.SaveChanges();
                return Json(new { message = "Success", Success = true });
            }
            return Json(new { message = "Unsuccess", Success = false });
        }


        public void ThongKe(string fromDate, string toDate)
        {
            var query = from o in db.Orders
                        join od in db.OrderDetails on o.Id equals od.OrderId
                        join p in db.Products
                        on od.ProductId equals p.Id
                        select new
                        {
                            CreatedDate = o.CreatedDate,
                            Quantity = od.Quantity,
                            Price = od.Price,
                            OriginalPrice = p.Price
                        };
            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime start = DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"));
                query = query.Where(x => x.CreatedDate >= start);
            }
            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime endDate = DateTime.ParseExact(toDate, "dd/MM/yyyy", CultureInfo.GetCultureInfo("vi-VN"));
                query = query.Where(x => x.CreatedDate < endDate);
            }
            var result = query.GroupBy(x => DbFunctions.TruncateTime(x.CreatedDate)).Select(r => new
            {
                Date = r.Key.Value,
                TotalBuy = r.Sum(x => x.OriginalPrice * x.Quantity), // tổng giá bán
                TotalSell = r.Sum(x => x.Price * x.Quantity) // tổng giá mua
            }).Select(x => new RevenueStatisticViewModel
            {
                Date = x.Date,
                Benefit = x.TotalSell - x.TotalBuy,
                Revenues = x.TotalSell
            });
        }
        public ActionResult ExportExcel(int orderId)
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Invoice");

            var ls = _orderViewModel.GetOrderDetailForExcel(orderId);

            // Thêm tiêu đề "Hóa đơn bán hàng"
            ws.Cell("A1").Value = "Hóa đơn bán hàng";
            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A1").Style.Font.FontSize = 18;
            ws.Range("A1:E1").Merge();

            // Lấy thông tin khách hàng từ danh sách ls
            string customerName = ls.FirstOrDefault()?.CustomerName;
            string customerAddress = ls.FirstOrDefault()?.Address;
            string customerPhone = ls.FirstOrDefault()?.Phone;

            // Thêm thông tin khách hàng
            ws.Cell("A3").Value = "Thông tin khách hàng";
            ws.Cell("A3").Style.Font.Bold = true;
            ws.Range("A3:E3").Merge();
            ws.Cell("A4").Value = "Tên khách hàng:";
            ws.Cell("A5").Value = "Địa chỉ:";
            ws.Cell("A6").Value = "Số điện thoại:";
            ws.Cell("B4").Value = customerName;
            ws.Cell("B5").Value = customerAddress;
            ws.Cell("B6").Value = customerPhone;

            // Thêm bảng thông tin sản phẩm
            ws.Cell("A8").Value = "STT";
            ws.Cell("B8").Value = "Tên sản phẩm";
            ws.Cell("C8").Value = "Số lượng";
            ws.Cell("D8").Value = "Đơn giá";
            ws.Cell("E8").Value = "Thành tiền";
            ws.Range("A8:E8").Style.Font.Bold = true;
            ws.Range("A8:E8").Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Range("A8:E8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            int row = 9;
            int stt = 1;
            foreach (var item in ls)
            {
                ws.Cell("A" + row).Value = stt;
                ws.Cell("B" + row).Value = item.ProductName;
                ws.Cell("C" + row).Value = item.Quantity;
                ws.Cell("D" + row).Value = item.Price;
                ws.Cell("E" + row).Value = item.Quantity * item.Price;
                row++;
                stt++;
            }

            // Tính tổng cộng
            ws.Cell("D" + row).Value = "Tổng cộng:";
            ws.Cell("E" + row).FormulaA1 = "=SUM(E9:E" + (row - 1) + ")";
            ws.Range("D" + row + ":E" + row).Style.Font.Bold = true;

            ws.Columns().AdjustToContents();

            string nameFile = "Invoice_" + DateTime.Now.Ticks + ".xlsx";
            string pathFile = Server.MapPath("~/Resources/ExportExcel/" + nameFile);
            wb.SaveAs(pathFile);

            return Json(nameFile, JsonRequestBehavior.AllowGet);
        }
       
    }
}