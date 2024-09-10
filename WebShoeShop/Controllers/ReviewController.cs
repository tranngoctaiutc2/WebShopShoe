using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShoeShop.Models;
using WebShoeShop.Models.EF;

namespace WebShoeShop.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Review
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult _Review(int productId)
        {
            ViewBag.ProductId = productId;
            var item = new ReviewProduct();
            if(User.Identity.IsAuthenticated)
            {
                var userStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
                var userManager = new UserManager<ApplicationUser>(userStore);
                var user = userManager.FindByName(User.Identity.Name);
                if(user != null)
                {
                    item.Email = user.Email;    
                    item.FullName = user.FullName;  
                    item.UserName = user.UserName;
                }
                return PartialView(item);
            }
            return PartialView();
        }
        [AllowAnonymous]
        public ActionResult _load_review(int productId) {
            var item = db.Reviews.Where(x => x.ProductId == productId).OrderByDescending(x=>x.Id).ToList();
            ViewBag.Count = item.Count;  
            return PartialView(item);
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult PostReview(ReviewProduct req)
        {
            if(ModelState.IsValid)
            { 
                req.CreateDate = DateTime.Now;
                db.Reviews.Add(req);
                db.SaveChanges();
                // Gửi yêu cầu Ajax để tải lại danh sách đánh giá mới nhất
                var item = db.Reviews.Where(x => x.ProductId == req.ProductId).OrderByDescending(x => x.Id).ToList();
                return PartialView("_load_review", item);
            }
            return Json(new { Success = false });
        }        
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}