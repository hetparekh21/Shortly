using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Shortly.Models;
using Microsoft.AspNet.Identity;
using System.Security;

namespace Shortly.Controllers
{
    public class UrlModelsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: UrlModels
        [Authorize]
        public ActionResult Index()
        {
            string user_id = User.Identity.GetUserId();
            return View(db.Url.Where(u => u.User_id == user_id).ToList());
        }

        // GET: UrlModels/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UrlModels urlModels = db.Url.Find(id);
            if (urlModels == null)
            {
                return HttpNotFound();
            }
            return View(urlModels);
        }

        // GET: UrlModels/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: UrlModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,RedirectTo,BackHalf,Qr,Auth,Active")] UrlModels urlModels)
        {

            if (ModelState.IsValid)
            {
                string user_id = User.Identity.GetUserId();
                urlModels.User_id = user_id;

                if (db.Url.Where(u => u.BackHalf == urlModels.BackHalf).FirstOrDefault() != default)
                {
                    Session["already_exists"] = "The Give back-half already exists , please choose another one";
                    return View();
                }

                if (urlModels.Auth)
                {
                    urlModels.Password = Request["password"];
                }

                db.Url.Add(urlModels);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(urlModels);
        }

        [Route("UrlRequestAccess/{id}")]
        [Authorize]
        public ActionResult UrlRequestAccess(int id)
        {
            UrlModels url = db.Url.Find(id);

            if(url == null)
            {
                return HttpNotFound();
            }

            List<RequestAccessModels> requestAccesses = db.RequestAccess.Where(r => r.Url.Id == url.Id).ToList();
            ViewData["url_id"] = url.Id;

            return View("~/Views/UrlModels/RequestAccess.cshtml",requestAccesses);
        }

        [Route("ChangeRequestStatus")]
        [Authorize]
        public ActionResult AccessStatus()
        {
            RequestAccessModels requestAccess = db.RequestAccess.Find(Convert.ToInt32(Request.Form["id"]));
            UrlModels url = db.Url.Find(Convert.ToInt32(Request.Form["url"]));
            requestAccess.Url = url;
            //System.Diagnostics.Debug.WriteLine("***********************"+Request.Form["id"]+"***********************");
            //System.Diagnostics.Debug.WriteLine("***********************"+ requestAccess.Email+ "***********************");
            //System.Diagnostics.Debug.WriteLine("***********************"+ requestAccess.Note+ "***********************");
            //System.Diagnostics.Debug.WriteLine("***********************"+ requestAccess.Granted+ "***********************");
            //System.Diagnostics.Debug.WriteLine("***********************"+requestAccess.Url+"***********************");
            if (requestAccess.Granted)
            {
                requestAccess.Granted = false;
            }
            else
            {
                // TODO : Send a mail with password
                SendMessage(requestAccess.Email,url);
                requestAccess.Granted = true;
            }

            db.Entry(requestAccess).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                    }
                }

            }

            return Json(new {status="updated"});


        }

        [Route("ChangeUrlStatus")]
        public ActionResult UrlStatus()
        {

            UrlModels url = db.Url.Find(Convert.ToInt32(Request.Form["id"]));

            url.Active = !url.Active ;
            db.Entry(url).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new { status = "updated" });

        }

        // GET: UrlModels/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            string user_id = User.Identity.GetUserId();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UrlModels urlModels = db.Url.Find(id);
            if (urlModels == null || urlModels.User_id != user_id)
            {
                return HttpNotFound();
            }

            ViewData["qr_url"] = GetRedirectUrl(Request.Url) + ":44306" + "/q/" + urlModels.BackHalf;

            Session["old_pass"] = urlModels.Password;
            return View(urlModels);
        }

        private string GetRedirectUrl(Uri url)
        {
            //var uri = new Uri(url);
            var domainURL = string.Format("{0}://{1}", url.Scheme, url.Host);
            return domainURL;
        }

        // POST: UrlModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "Id,RedirectTo,BackHalf,Qr,Auth,Password,Active")] UrlModels urlModels)
        {
            if (ModelState.IsValid)
            {
                string user_id = User.Identity.GetUserId();
                urlModels.User_id = user_id;

                if ((string)Session["old_pass"] != urlModels.Password)
                {
                    System.Diagnostics.Debug.WriteLine("*******************************no");
                    db.Database.ExecuteSqlCommand("UPDATE RequestAccess SET Granted=0 WHERE Url_Id="+urlModels.Id);
                }

                db.Entry(urlModels).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(urlModels);
        }

        // GET: UrlModels/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            string user_id = User.Identity.GetUserId();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UrlModels urlModels = db.Url.Find(id);
            if (urlModels == null || urlModels.User_id != user_id)
            {
                return HttpNotFound();
            }
            return View(urlModels);
        }

        // POST: UrlModels/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UrlModels urlModels = db.Url.Find(id);
            db.Url.Remove(urlModels);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public void SendMessage(string toAddress, UrlModels url)
        {

            string send_url = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Authority + "/r/" + url.BackHalf;
            MailMessage newMessage = new MailMessage("admin@gmail.com", toAddress);

            newMessage.IsBodyHtml = true;
            newMessage.Subject = "Access Granted";
            newMessage.Body = "<p>Congratulations, You've been granted access to this <a href='" + send_url + "' >url</a> </p> <br> <p> Here is the password to the URL: </p><b>" + url.Password + "</b> ";

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Send(newMessage);

        }

    }
}
