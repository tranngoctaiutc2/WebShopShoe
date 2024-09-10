using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShoeShop.Models;
using WebShoeShop.Models.EF;

namespace WebShoeShop.Areas.Admin.Controllers
{
    [Authorize(Roles ="Admin,Employee")]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Home
        public ActionResult Index()
        {
            int totalProducts = db.Products.Count();
            int totalOrders = db.Orders.Count();
            decimal totalRevenue = db.Orders.Sum(o => o.TotalAmount);
            decimal totalAccess = db.ThongKes.Sum(o=>o.SoTruyCap);
           int totalUser = db.Users.Count();
          
            ViewBag.TotalProducts = totalProducts;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalAccess = totalAccess;
            ViewBag.TotalUser = totalUser;
            return View();
            
        }
    }
}