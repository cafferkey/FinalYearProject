using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Google.Apis.Books.v1;
using Google.Apis.Services;
using FYP.Models;

namespace FYP.Controllers {

    public class HomeController : Controller {

        // Database Accessor
        private bookclubEntities db = new bookclubEntities();

        // Create API Service
        BooksService service = new BooksService(new BaseClientService.Initializer {
            ApplicationName = "FYP 2020",
            ApiKey = "AIzaSyDkQjaKLb-irxBxA6b1BVG0f69JN0xHAJI",
            GZipEnabled = true,
        });

        String[] bestSellersNY = new String[] { "51YXwQEACAAJ", "mewJwwEACAAJ", "YpTwxQEACAAJ", "v3nZvQEACAAJ", "d33HDwAAQBAJ" };

        public ActionResult Index(String SearchString) {
            GenerateList(bestSellersNY).Wait();

            if (SearchString != null) {
                return RedirectToAction("SearchResults", new { id = SearchString });
            } else {

                return View();
            }
        }


        public ActionResult bookinfo(String id) {

            CreateBook(id).Wait();

            return View();
        }

        public ActionResult Review(String id) {

            if (id == null) {

                return RedirectToAction("Login");

            } else {
                CreateBook(id).Wait();
                return View();
            }
        }

        public void addReview(String rev, String gbid, int uid) {
            userbook ub = db.userbooks.Find(uid, gbid);

            if (ub != null) {

                ub.reviewbody = rev;

            } else {

                NewUserBook(gbid, uid, true);
                userbook ub1 = db.userbooks.Find(uid, gbid);
                ub1.reviewbody = rev;
            }


            db.SaveChanges();
        }

        public ActionResult MyClubs(int? id) {

            if (id == null) {

                return RedirectToAction("Login");

            } else {

                var clubs = (from club in db.clubs
                             join memb in db.members on club.clubid equals memb.clubid
                             where memb.userid == id
                             select club).ToList();

                return View(clubs);
            }
        }

        public ActionResult MyBooks(int id) {


            if (id == null) {

                return RedirectToAction("Login");

            } else {

                ViewBag.booksRead = (from usb in db.userbooks
                                     where (usb.userid == id) && (usb.isread == true)
                                     select usb.gbid).ToList();


                ViewBag.booksToRead = (from usb in db.userbooks
                                       where (usb.userid == id) && (usb.isread == false)
                                       select usb.gbid).ToList();

                List<String> list = new List<string>();

                foreach (var shite in ViewBag.booksRead) {
                    list.Add(shite);
                }

                foreach (var moreshite in ViewBag.booksToRead) {
                    list.Add(moreshite);
                }

                GenerateList(list.ToArray()).Wait();

                return View();
            }
        }

        public ActionResult SearchResults(String id) {

            if (id == null) {

                return RedirectToAction("Login");

            } else {
                ViewBag.searched = id;
                BookSearch(id).Wait();
                return View();
            }
        }

        public ActionResult userinfo(int id) {

            user u = db.users.Find(id);
            return View(u);
        }

        [HttpPost]
        public void NewUserBook(string id, int uid, bool isRead) {
            userbook ub = new userbook {
                gbid = id,
                userid = uid,
                isread = isRead
            };

            var amount = (from usb in db.userbooks
                          where (usb.userid == uid) && (usb.gbid == id)
                          select usb).Count();

            if (amount == 0) { 
                db.userbooks.Add(ub);
                db.SaveChanges();
            }
        }

        [HttpPost]
        public void NewClubBook(string id, int clubid, bool isRead) {
            clubbook cb = new clubbook {
                gbid = id,
                clubid = clubid,
                isread = isRead
            };

            var amount = (from clb in db.clubbooks
                          where (clb.clubid == clubid) && (clb.gbid == id)
                          select clb).Count();

            if (amount == 0) {
                db.clubbooks.Add(cb);
                db.SaveChanges();
            }
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


        private async Task CreateBook(String gbid) {


            // Perform API Get request
            var vservice = await service.Volumes.Get(gbid).ExecuteAsync().ConfigureAwait(false);

            // Pass book metadata to ViewBag as a list of strings
            List<String> bookDetails = new List<string>();

            bookDetails.Add(vservice.VolumeInfo.Title);
            bookDetails.Add(vservice.VolumeInfo.Authors.First());
            bookDetails.Add(vservice.VolumeInfo.Publisher);
            bookDetails.Add(vservice.VolumeInfo.PublishedDate);
            bookDetails.Add(vservice.VolumeInfo.Description);
            bookDetails.Add(vservice.VolumeInfo.ImageLinks.Thumbnail);
            bookDetails.Add(vservice.Id);

            ViewBag.bookDetails = bookDetails;
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
                    


        // Registration
        [HttpGet]
        public ActionResult Register() {
            return View();
        }

        [HttpPost]
        public ActionResult Register(user us) {

            user u = new user();

            try {
                u.fullname = us.fullname;
                u.password = us.password;
                u.email = us.email;
                db.users.Add(u);
                db.SaveChanges();

                Session["userid"] = u.userid;
                Session["email"] = u.email;
                Session["name"] = u.fullname;
                return RedirectToAction("Index");
            }
            catch (Exception) {

                ViewBag.msg = "Data Could not be inserted......";
            }

            return View();
        }

        [HttpGet]
        public ActionResult Logout() {
            Session.Abandon();
            Session.RemoveAll();
            return RedirectToAction("Index");
        }

        // Login
        [HttpGet]
        public ActionResult Login() {
            return View();
        }

        [HttpPost]
        public ActionResult Login(user us) {

            user u = db.users.Where(x => x.email == us.email && x.password == us.password).SingleOrDefault();
            if (u == null) {
                ViewBag.msg = "Invalid Email or Password";
            } else {
                Session["userid"] = u.userid;
                Session["email"] = u.email;
                Session["name"] = u.fullname;
                return RedirectToAction("Index");
            }
            return View();
        }
    }






}