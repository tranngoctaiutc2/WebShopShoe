using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShoeShop.Models;
using WebShoeShop.Models.EF;

namespace WebShoeShop.Controllers
{
    public class SearchController : Controller
    {
       private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Search
        public ActionResult Index(String Searchtext)
        {
            IEnumerable<Product> items = db.Products.OrderByDescending(x => x.Id);
            if (!string.IsNullOrEmpty(Searchtext))
            {
                string searchTextLower = Searchtext.ToLowerInvariant();
                items = items.Where(x => x.Alias.IndexOf(searchTextLower, StringComparison.OrdinalIgnoreCase) != -1 || x.Title.IndexOf(searchTextLower, StringComparison.OrdinalIgnoreCase) != -1);
            }
            return View(items);
        }
    }
}