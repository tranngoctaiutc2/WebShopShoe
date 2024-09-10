using Microsoft.AspNet.SignalR.Messaging;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using WebShoeShop.Models;
using WebShoeShop.Models.EF;

namespace WebShoeShop.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        // GET: Wishlist
        public ActionResult Index(int? page)
        {
            var pageSize = 3;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<Wishlist> items = db.Wishlists.Where(x => x.UserName == User.Identity.Name).OrderByDescending(x =>x.CreateDate);
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;
            return View(items);
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult PostWishlist(int ProductId)
        {
            if(Request.IsAuthenticated == false)
            {
                return Json(new { Success = false ,Message = "Bạn chưa đăng nhập" });
            }
           var checkitem = db.Wishlists.FirstOrDefault(x=>x.ProductId == ProductId && x.UserName == User.Identity.Name);
            if (checkitem != null)
            {
                return Json(new { Success = false, Message = "Sản phẩm đã được yêu thích" });
            }
            var item = new Wishlist();
            item.ProductId = ProductId;
            item.UserName = User.Identity.Name;
            item.CreateDate = DateTime.Now;
            db.Wishlists.Add(item);
            db.SaveChanges();
            return Json(new {Success = true});
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult PostDeleteWishlist(int ProductId)
        {
            var checkitem = db.Wishlists.FirstOrDefault(x => x.ProductId == ProductId && x.UserName == User.Identity.Name);
            if (checkitem != null)
            {
                db.Wishlists.Remove(checkitem);
                db.SaveChanges();
                return Json(new { Success = true, Message = "Xóa thành công." });
            }
            return Json(new { Success = false, Message = "Xóa thất bại." });   
        }
        private ApplicationDbContext db = new ApplicationDbContext();
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}