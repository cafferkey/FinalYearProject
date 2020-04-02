using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FYP.Models;
using Google.Apis.Books.v1;
using Google.Apis.Services;
using System.Threading.Tasks;

namespace FYP.Views
{
    public class meetingsController : Controller
    {
        private bookclubEntities db = new bookclubEntities();

        BooksService service = new BooksService(new BaseClientService.Initializer {
            ApplicationName = "FYP 2020",
            ApiKey = "AIzaSyDkQjaKLb-irxBxA6b1BVG0f69JN0xHAJI",
            GZipEnabled = true,
        });

        // GET: meetings
        public ActionResult Index()
        {
            var meetings = db.meetings.Include(m => m.club);
            return View(meetings.ToList());
        }

        // GET: meetings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            meeting meeting = db.meetings.Find(id);
            if (meeting == null)
            {
                return HttpNotFound();
            }
            return View(meeting);
        }

        // GET: meetings/Create
        public ActionResult Create(int cid)
        {
            String[] unreadList = (from book in db.clubbooks
                              where (book.clubid == cid) && (book.isread == false)
                              select book.gbid).ToArray();

            GetBookTitles(unreadList).Wait();

            ViewBag.unreadIdList = unreadList;
            ViewBag.cid = cid;

            ViewBag.clubid = new SelectList(db.clubs, "clubid", "clubname");

            return View();
        }

        private async Task GetBookTitles(String[] input) {

            List<String> unreadTitles = new List<string>();

            for (var i = 0; i < input.Length; i++) {
                var vservice = await service.Volumes.Get(input[i]).ExecuteAsync().ConfigureAwait(false);
                unreadTitles.Add(vservice.VolumeInfo.Title);
            }

            ViewBag.unreadTitles = unreadTitles;
        }

        // POST: meetings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "meetingid,clubid,meetingname,description,bookid,meetingtime")] meeting meeting)
        {
            if (ModelState.IsValid)
            {
                db.meetings.Add(meeting);
                db.SaveChanges();
                return RedirectToAction("Details", "clubs", new { id = meeting.clubid });
            }

            ViewBag.clubid = new SelectList(db.clubs, "clubid", "clubname", meeting.clubid);
            return View(meeting);
        }

        // GET: meetings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            meeting meeting = db.meetings.Find(id);
            if (meeting == null)
            {
                return HttpNotFound();
            }
            ViewBag.clubid = new SelectList(db.clubs, "clubid", "clubname", meeting.clubid);
            return View(meeting);
        }

        // POST: meetings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "meetingid,clubid,meetingname,description,bookid,meetingtime")] meeting meeting)
        {
            if (ModelState.IsValid)
            {
                db.Entry(meeting).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.clubid = new SelectList(db.clubs, "clubid", "clubname", meeting.clubid);
            return View(meeting);
        }

        // GET: meetings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            meeting meeting = db.meetings.Find(id);
            if (meeting == null)
            {
                return HttpNotFound();
            }
            return View(meeting);
        }

        // POST: meetings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            meeting meeting = db.meetings.Find(id);
            db.meetings.Remove(meeting);
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
