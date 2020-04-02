using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Google.Apis.Books.v1;
using Google.Apis.Services;
using FYP.Models;

namespace FYP.Views
{
    public class clubsController : Controller
    {
        private bookclubEntities db = new bookclubEntities();

        BooksService service = new BooksService(new BaseClientService.Initializer {
            ApplicationName = "FYP 2020",
            ApiKey = "AIzaSyDkQjaKLb-irxBxA6b1BVG0f69JN0xHAJI",
            GZipEnabled = true,
        });

        // GET: clubs
        public ActionResult Index()
        {
           
            return View(db.clubs.ToList());
        }


        public ActionResult ClubResults(int id, String sid, String SearchString) {

            var savedBooksTrue = (from clb in db.clubbooks
                                  where (clb.clubid == id) && (clb.isread == true)
                                  select clb).ToList();

            var savedBooksFalse = (from clb in db.clubbooks
                                  where (clb.clubid == id) && (clb.isread == false)
                                  select clb).ToList();

            var list = (from clb in db.clubbooks
                        where clb.clubid == id
                        select clb.gbid).ToArray();

            GenerateClubList(list).Wait();

            ViewBag.clubBooksRead = savedBooksTrue;
            ViewBag.clubBooksToRead = savedBooksFalse;

            ViewBag.searched = sid;
            BookSearch(sid).Wait();



            club club = db.clubs.Find(id);
            return View(club);
            
        }

        public ActionResult ClubResultsUnR(int id, String sid, String SearchString) {

            var savedBooksTrue = (from clb in db.clubbooks
                                  where (clb.clubid == id) && (clb.isread == true)
                                  select clb).ToList();

            var savedBooksFalse = (from clb in db.clubbooks
                                   where (clb.clubid == id) && (clb.isread == false)
                                   select clb).ToList();

            var list = (from clb in db.clubbooks
                        where clb.clubid == id
                        select clb.gbid).ToArray();

            GenerateClubList(list).Wait();

            ViewBag.clubBooksRead = savedBooksTrue;
            ViewBag.clubBooksToRead = savedBooksFalse;

            ViewBag.searched = sid;
            BookSearch(sid).Wait();



            club club = db.clubs.Find(id);
            return View(club);

        }


        private async Task BookSearch(String q) {

            // Perform Search
            var vservice = await service.Volumes.List(q).ExecuteAsync().ConfigureAwait(false);

            List<String> inputIds = new List<string>();

            foreach (var item in vservice.Items) {
                inputIds.Add(item.Id);
            }

            GenerateList(inputIds.ToArray()).Wait();

        }

               
        private async Task GenerateList(String[] input) {

            List<List<String>> mainBookList = new List<List<String>>();

            for (var i = 0; i < input.Length; i++) {

                var vservice = await service.Volumes.Get(input[i]).ExecuteAsync().ConfigureAwait(false);

                List<String> bookDetails = new List<string>();

                bookDetails.Add(vservice.VolumeInfo.Title);
                bookDetails.Add(vservice.VolumeInfo.Authors.First());
                bookDetails.Add(vservice.VolumeInfo.Publisher);
                bookDetails.Add(vservice.VolumeInfo.PublishedDate);
                bookDetails.Add(vservice.VolumeInfo.Description);
                bookDetails.Add(vservice.VolumeInfo.ImageLinks.Thumbnail);
                bookDetails.Add(vservice.Id);

                mainBookList.Add(bookDetails);
            }

            ViewBag.booklist = mainBookList;
        }


        private async Task GenerateClubList(String[] input) {

            List<List<String>> mainBookList = new List<List<String>>();

            for (var i = 0; i < input.Length; i++) {

                var vservice = await service.Volumes.Get(input[i]).ExecuteAsync().ConfigureAwait(false);

                List<String> bookDetails = new List<string>();

                bookDetails.Add(vservice.VolumeInfo.Title);
                bookDetails.Add(vservice.VolumeInfo.Authors.First());
                bookDetails.Add(vservice.VolumeInfo.Publisher);
                bookDetails.Add(vservice.VolumeInfo.PublishedDate);
                bookDetails.Add(vservice.VolumeInfo.Description);
                bookDetails.Add(vservice.VolumeInfo.ImageLinks.Thumbnail);
                bookDetails.Add(vservice.Id);

                mainBookList.Add(bookDetails);
            }

            ViewBag.clubBooklist = mainBookList;
        }






        // GET: clubs/Details/5
        public ActionResult Details(int? id, String SearchString)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var savedBooksTrue = (from clb in db.clubbooks
                                  where (clb.clubid == id) && (clb.isread == true)
                                  select clb).ToList();

            var savedBooksFalse = (from clb in db.clubbooks
                                   where (clb.clubid == id) && (clb.isread == false)
                                   select clb).ToList();

            var list = (from clb in db.clubbooks
                        where clb.clubid == id
                        select clb.gbid).ToArray();

            GenerateClubList(list).Wait();

            ViewBag.clubBooksRead = savedBooksTrue;
            ViewBag.clubBooksToRead = savedBooksFalse;

            club club = db.clubs.Find(id);
            if (club == null)
            {
                return HttpNotFound();
            }

            if (SearchString != null) {
                return RedirectToAction("ClubResults", new { id = id,  sid = SearchString });
            } else {
                return View(club);
            }
        }


        public ActionResult AddMembers(int id) {
            club club = db.clubs.Find(id);
            return View(club);
        }


        [HttpPost]
        public ActionResult AddMembers(int id, String memberemail) {

            var amount = (from user in db.users
                          where user.email == memberemail
                          select user).Count();

            if(amount != 0) {

                int uid = (from user in db.users
                           where user.email == memberemail
                           select user.userid).FirstOrDefault();

                member m = new member();
                m.userid = uid;
                m.clubid = id;

                db.members.Add(m);
                db.SaveChanges();
            }

            return RedirectToAction("Details", "clubs", new { id = id });
        }

        // GET: clubs/Create
        public ActionResult Create()
        {
            ViewBag.members = new SelectList(db.users, "userid", "password");
            return View();
        }

        // POST: clubs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "clubid,clubname,creator,admins,description")] club club)
        {
            club.creator = (int)Session["userid"];
            if (ModelState.IsValid)
            {
                db.clubs.Add(club);

                member m = new member();
                m.clubid = club.clubid;
                m.userid = (int)Session["userid"];
                db.members.Add(m);

                db.SaveChanges();
                return RedirectToAction("Details", "clubs", new { id = club.clubid });
            }

            ViewBag.members = new SelectList(db.users, "userid", "fullname", club.members);
            return View(club);
        }

        // GET: clubs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            club club = db.clubs.Find(id);
            if (club == null)
            {
                return HttpNotFound();
            }
            ViewBag.members = new SelectList(db.users, "userid", "fullname", club.members);
            return View(club);
        }

        // POST: clubs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "clubid,clubname,members,creator,admins,booksread,bookstoread,meetings")] club club)
        {
            if (ModelState.IsValid)
            {
                db.Entry(club).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.members = new SelectList(db.users, "userid", "fullname", club.members);
            return View(club);
        }

        // GET: clubs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            club club = db.clubs.Find(id);
            if (club == null)
            {
                return HttpNotFound();
            }
            return View(club);
        }

        // POST: clubs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            club club = db.clubs.Find(id);
            db.clubs.Remove(club);
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
