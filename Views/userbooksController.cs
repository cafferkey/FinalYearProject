using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FYP.Models;

namespace FYP.Views
{
    public class userbooksController : Controller
    {
        private bookclubEntities db = new bookclubEntities();

        // GET: userbooks
        public ActionResult Index()
        {
            var userbooks = db.userbooks.Include(u => u.user);
            return View(userbooks.ToList());
        }

        // GET: userbooks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            userbook userbook = db.userbooks.Find(id);
            if (userbook == null)
            {
                return HttpNotFound();
            }
            return View(userbook);
        }

        // GET: userbooks/Create
        public ActionResult Create()
        {
            ViewBag.userid = new SelectList(db.users, "userid", "password");
            return View();
        }

        // POST: userbooks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "userid,gbid,isread,reviewbody,urating")] userbook userbook)
        {
            if (ModelState.IsValid)
            {
                db.userbooks.Add(userbook);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.userid = new SelectList(db.users, "userid", "password", userbook.userid);
            return View(userbook);
        }

        // GET: userbooks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            userbook userbook = db.userbooks.Find(id);
            if (userbook == null)
            {
                return HttpNotFound();
            }
            ViewBag.userid = new SelectList(db.users, "userid", "password", userbook.userid);
            return View(userbook);
        }

        // POST: userbooks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "userid,gbid,isread,reviewbody,urating")] userbook userbook)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userbook).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.userid = new SelectList(db.users, "userid", "password", userbook.userid);
            return View(userbook);
        }

        // GET: userbooks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            userbook userbook = db.userbooks.Find(id);
            if (userbook == null)
            {
                return HttpNotFound();
            }
            return View(userbook);
        }

        // POST: userbooks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            userbook userbook = db.userbooks.Find(id);
            db.userbooks.Remove(userbook);
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
    }
}
