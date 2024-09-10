using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShoeShop.Models.EF;
using WebShoeShop.Models;

namespace WebShoeShop.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SettingSystemController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/SettingSystem
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Partial_Setting()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult AddSetting(SettingSystemViewModel req)
        {
            SystemSetting set = null;
            var checkTitle = db.SystemSettings.FirstOrDefault(x => x.SettingKey.Contains("SettingTitle"));
            if (checkTitle == null)
            {
                set = new SystemSetting();
                set.SettingKey = "SettingTitle";
                set.SettingValue = req.SettingTitle;
                db.SystemSettings.Add(set);
            }
            else
            {
                checkTitle.SettingValue = req.SettingTitle;
                db.Entry(checkTitle).State = System.Data.Entity.EntityState.Modified;
            }
            //logo
            var checkLogo = db.SystemSettings.FirstOrDefault(x => x.SettingKey.Contains("SettingLogo"));
            if (checkLogo == null)
            {
                set = new SystemSetting();
                set.SettingKey = "SettingLogo";
                set.SettingValue = req.SettingLogo;
                db.SystemSettings.Add(set);
            }
            else
            {
                checkLogo.SettingValue = req.SettingLogo;
                db.Entry(checkLogo).State = System.Data.Entity.EntityState.Modified;
            }
            //Email
            var email = db.SystemSettings.FirstOrDefault(x => x.SettingKey.Contains("SettingEmail"));
            if (email == null)
            {
                set = new SystemSetting();
                set.SettingKey = "SettingEmail";
                set.SettingValue = req.SettingEmail;
                db.SystemSettings.Add(set);
            }
            else
            {
                email.SettingValue = req.SettingEmail;
                db.Entry(email).State = System.Data.Entity.EntityState.Modified;
            }
            //Hotline
            var Hotline = db.SystemSettings.FirstOrDefault(x => x.SettingKey.Contains("SettingHotline"));
            if (Hotline == null)
            {
                set = new SystemSetting();
                set.SettingKey = "SettingHotline";
                set.SettingValue = req.SettingHotline;
                db.SystemSettings.Add(set);
            }
            else
            {
                Hotline.SettingValue = req.SettingHotline;
                db.Entry(Hotline).State = System.Data.Entity.EntityState.Modified;
            }
            //TitleSeo
            var TitleSeo = db.SystemSettings.FirstOrDefault(x => x.SettingKey.Contains("SettingTitleSeo"));
            if (TitleSeo == null)
            {
                set = new SystemSetting();
                set.SettingKey = "SettingTitleSeo";
                set.SettingValue = req.SettingTitleSeo;
                db.SystemSettings.Add(set);
            }
            else
            {
                TitleSeo.SettingValue = req.SettingTitleSeo;
                db.Entry(TitleSeo).State = System.Data.Entity.EntityState.Modified;
            }
            //DessSeo
            var DessSeo = db.SystemSettings.FirstOrDefault(x => x.SettingKey.Contains("SettingDesSeo"));
            if (DessSeo == null)
            {
                set = new SystemSetting();
                set.SettingKey = "SettingDesSeo";
                set.SettingValue = req.SettingDesSeo;
                db.SystemSettings.Add(set);
            }
            else
            {
                DessSeo.SettingValue = req.SettingDesSeo;
                db.Entry(DessSeo).State = System.Data.Entity.EntityState.Modified;
            }
            //KeySeo
            var KeySeo = db.SystemSettings.FirstOrDefault(x => x.SettingKey.Contains("SettingKeySeo"));
            if (KeySeo == null)
            {
                set = new SystemSetting();
                set.SettingKey = "SettingKeySeo";
                set.SettingValue = req.SettingKeySeo;
                db.SystemSettings.Add(set);
            }
            else
            {
                KeySeo.SettingValue = req.SettingKeySeo;
                db.Entry(KeySeo).State = System.Data.Entity.EntityState.Modified;
            }
            //Banner
            var Banner = db.SystemSettings.FirstOrDefault(x => x.SettingKey.Contains("SettingBanner"));
            if (Banner == null)
            {
                set = new SystemSetting();
                set.SettingKey = "SettingBanner";
                set.SettingValue = req.SettingBanner;
                db.SystemSettings.Add(set);
            }
            else
            {
                Banner.SettingValue = req.SettingBanner;
                db.Entry(Banner).State = System.Data.Entity.EntityState.Modified;
            }
            //Address
            var address = db.SystemSettings.FirstOrDefault(x => x.SettingKey.Contains("SettingAddress"));
            if (address == null)
            {
                set = new SystemSetting();
                set.SettingKey = "SettingAddress";
                set.SettingValue = req.SettingAddress;
                db.SystemSettings.Add(set);
            }
            else
            {
                address.SettingValue = req.SettingAddress;
                db.Entry(address).State = System.Data.Entity.EntityState.Modified;
            }
            //Facebook
            var fb = db.SystemSettings.FirstOrDefault(x => x.SettingKey.Contains("SettingFacebook"));
            if (fb == null)
            {
                set = new SystemSetting();
                set.SettingKey = "SettingFacebook";
                set.SettingValue = req.SettingFacebook;
                db.SystemSettings.Add(set);
            }
            else
            {
                fb.SettingValue = req.SettingFacebook;
                db.Entry(fb).State = System.Data.Entity.EntityState.Modified;
            }
            //Zalo
            var zalo = db.SystemSettings.FirstOrDefault(x => x.SettingKey.Contains("SettingZalo"));
            if (zalo == null)
            {
                set = new SystemSetting();
                set.SettingKey = "SettingZalo";
                set.SettingValue = req.SettingZalo;
                db.SystemSettings.Add(set);
            }
            else
            {
                zalo.SettingValue = req.SettingZalo;
                db.Entry(zalo).State = System.Data.Entity.EntityState.Modified;
            }
            //Youtube
            var yt = db.SystemSettings.FirstOrDefault(x => x.SettingKey.Contains("SettingYoutube"));
            if (yt == null)
            {
                set = new SystemSetting();
                set.SettingKey = "SettingYoutube";
                set.SettingValue = req.SettingYoutube;
                db.SystemSettings.Add(set);
            }
            else
            {
                yt.SettingValue = req.SettingYoutube;
                db.Entry(yt).State = System.Data.Entity.EntityState.Modified;
            }
            db.SaveChanges();

            return View("Partial_Setting");
        }
    }
}