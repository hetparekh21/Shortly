using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shortly.Models;
using Microsoft.AspNet.Identity;

namespace Shortly.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }

        [Route("DashBoard")]
        [Authorize]
        public ActionResult DashBoard()
        {
            string user_id = User.Identity.GetUserId();

            ViewData["your_urls"] = db.Url.Where(u => u.User_id == user_id).Count();
            ViewData["qr_codes"] = db.Url.Where(u => u.User_id == user_id && u.Qr == true).Count();
            ViewData["pending_requests"] = db.RequestAccess.Where(ra => ra.Url.User_id == user_id && ra.Granted == false).Count();
            ViewData["total_clicks"] = db.Stats.Where(st => st.Url.User_id == user_id).Count();

            int currentYear = DateTime.Now.Year;

            //var clicks = db.Stats
            //.Where(s => s.HitAt.Year == currentYear && s.isQR == false).GroupBy(s => new { Month = s.HitAt.Month })
            //.Select(g => new
            //{
            //    Month = g.Key.Month,
            //    TotalCount = g.Count()
            //})
            //.ToList();


            //var scans = db.Stats.Where(s => s.HitAt.Year == currentYear && s.isQR == true).GroupBy(s => new { Month = s.HitAt.Month })
            //.Select(g => new
            //{
            //    Month = g.Key.Month,
            //    TotalCount = g.Count()
            //})
            //.ToList();

            //ViewData["clicks"] = clicks;
            //ViewData["scans"] = scans;

            var stats = db.Stats.Where(s => s.Url.User_id == user_id).OrderByDescending(s => s.HitAt).Take(5).ToList();


            return View(stats);
        }

        [Route("getClicks")]
        public ActionResult getClicks()
        {
            string user_id = User.Identity.GetUserId();

            var clicks = db.Stats.Where(u => u.Url.User_id == user_id).GroupBy(u => new { url = u.Url.BackHalf })
                .Select(g => new
                {
                    url = g.Key,
                    count = g.Count()
                })
            .ToList();

            return Json(clicks,JsonRequestBehavior.AllowGet);

        }

    }
}