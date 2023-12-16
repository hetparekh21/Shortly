using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shortly.Models;
using System.Data.Entity;

namespace Shortly.Controllers
{
    public class RouteController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Route
        [HttpGet]
        [AllowAnonymous]
        [Route("r/{back_half}")]
        [Route("q/{back_half}")]
        public ActionResult Index(string back_half)
        {

            UrlModels url = GetUrlModel(back_half);
            string url_type = GetUrlType();

            if (url == null || url.Active == false || !url.Qr && url_type == "q")
            {
                // retrun a 404 , no such url found
                return HttpNotFound();
            }

            if (url.Auth)
            {
                ViewData["back_half"] = back_half;
                ViewData["url_type"] = url_type;
                return View("~/Views/UrlModels/UrlAuth.cshtml");
            }

            if (url.User_id != null && url.User_id != "")
            {
                AddStat(url, url_type);
            }


            var domainURL = url.RedirectTo;
            //System.Diagnostics.Debug.WriteLine(location);

            return Redirect(domainURL);

            //return new RedirectResult(url.BackHalf);

        }

        [HttpPost]
        [AllowAnonymous]
        [Route("r/{back_half}")]
        [Route("q/{back_half}")]
        public ActionResult Auth(string back_half)
        {

            UrlModels url = db.Url.Where(u => u.BackHalf == back_half).First();
            string url_type = GetUrlType();
            var domainURL = url.RedirectTo;

            if (url == null)
            {
                // retrun a 404 , no such url found
                return HttpNotFound();
            }

            var password = Request.Form["password"];

            if(!url.Auth || url.Password == password)
            {
                AddStat(url,url_type);
                return Redirect(domainURL);
            }
            else
            {
                ViewData["error"] = "Incorrect Password";
                ViewData["url_type"] = url_type;
                ViewData["back_half"] = back_half;
                return View("~/Views/UrlModels/UrlAuth.cshtml");
            }

        }

        [HttpGet]
        [AllowAnonymous]
        [Route("RequestAccess/{back_half}")]
        public ActionResult RequestAccess(string back_half)
        {
            UrlModels url = GetUrlModel(back_half);

            if(url == null)
            {
                return HttpNotFound();
            }

            // TODO : redirect to get email and note 
            //System.Diagnostics.Debug.WriteLine("****************"+back_half+"******************");
            //ViewBag.Message = "back_half";
            ViewData["back_half"] = back_half;
            return View("~/Views/Route/RequestAccess.cshtml");

        }

        [HttpPost]
        [AllowAnonymous]
        [Route("RequestAccess/{back_half}")]
        public ActionResult RequestAccessPost(string back_half)
        {
            UrlModels url = GetUrlModel(back_half);

            var email = Request.Form["email"];
            var note = Request.Form["note"];

            if (db.RequestAccess.Where(r => r.Email == email).FirstOrDefault() != default)
            {
                RequestAccessModels requestAccess = db.RequestAccess.Where(r => r.Email == email).First();
                requestAccess.Note = note;
                db.Entry(requestAccess).State = EntityState.Modified;
            }
            else
            {
                RequestAccessModels requestAccess = new RequestAccessModels();
                requestAccess.Email = email;
                requestAccess.Note = note;
                requestAccess.Url = url;
                db.RequestAccess.Add(requestAccess);
            }

            db.SaveChanges();

            // TODO : redirect to request submitted succesfully page 

            return View("~/Views/Route/RequestSuccesful.cshtml");

        }

        private UrlModels GetUrlModel(string back_half)
        {
            UrlModels url = db.Url.Where(u => u.BackHalf == back_half).FirstOrDefault();
            return url;
        }

        private void AddStat(UrlModels url, string url_type = "r")
        {
            string ip = Request.UserHostAddress;
            string browser = Request.Browser.Browser;
            string device_type = Request.Browser.Platform;
            string location = CountryModels.getCountry(IP3Country.CountryLookup.LookupIPStr("152.58.60.251"));

            StatsModels stats = new StatsModels();
            stats.Url = url;
            stats.Location = "Unknown";
            stats.IpAddress = ip;
            stats.BrowserType = browser;
            stats.DeviceType = device_type;

            if (url_type == "q")
            {
                stats.isQR = true;
            }

            if (location != null)
            {
                stats.Location = location;
            }
            stats.HitAt = DateTime.UtcNow;

            db.Stats.Add(stats);
            db.SaveChanges();

        }

        //private string GetRedirectUrl(string url)
        //{
        //    var uri = new Uri(url);
        //    var domainURL = string.Format("{0}://{1}", uri.Scheme, uri.Host);
        //    return domainURL;
        //}

        private string GetUrlType()
        {
            return Request.Path.Substring(1, 1);
        }

        // GET: Route/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        // GET: Route/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Route/Create
        //[HttpPost]
        //public ActionResult Create(FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        // GET: Route/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: Route/Edit/5
        //[HttpPost]
        //public ActionResult Edit(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        // GET: Route/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        // POST: Route/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //private static string GetIPAddress(HttpRequestBase request)
        //{
        //    string ip;
        //    try
        //    {
        //        ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        //        if (!string.IsNullOrEmpty(ip))
        //        {
        //            if (ip.IndexOf(",") > 0)
        //            {
        //                string[] ipRange = ip.Split(',');
        //                int le = ipRange.Length - 1;
        //                ip = ipRange[le];
        //            }
        //        }
        //        else
        //        {
        //            ip = request.UserHostAddress;
        //        }
        //    }
        //    catch { ip = null; }

        //    return ip;
        //}
    }
}
