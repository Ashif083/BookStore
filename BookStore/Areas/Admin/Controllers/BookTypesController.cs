using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BookTypesController : Controller
    {
        private readonly ApplicationDbContext _db;
        public BookTypesController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var obj = _db.Booktypes.ToList();
            return View(obj);
        }
        //Get
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BookTypes book)
        {
            if (ModelState.IsValid)
            {
                var checkBook=_db.Booktypes.FirstOrDefault(x=>x.BookType==book.BookType);
                if(checkBook!=null)
                {
                    ViewBag.message = "This Genre is already existed";
                    return View(book);
                }
                _db.Booktypes.Add(book);
                _db.SaveChanges();
                TempData["save"] = "New Genre has been Created";
                return RedirectToAction("Index");
            }
            return View(book);
        }
        //Get
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var obj = _db.Booktypes.FirstOrDefault(x => x.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }
        [HttpPost]
        public IActionResult Edit(BookTypes book)
        {

            if (ModelState.IsValid)
            {
                var checkBook = _db.Booktypes.FirstOrDefault(x => x.BookType == book.BookType);
                if (checkBook != null)
                {
                    ViewBag.message = "This Genre is already existed";
                    return View(book);
                }
                _db.Booktypes.Update(book);
                _db.SaveChanges();
                TempData["edit"] = "Genre has been Updated";
                return RedirectToAction("Index");
            }
            return View(book);
        }
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var obj = _db.Booktypes.FirstOrDefault(x => x.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }
        //Get
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var obj = _db.Booktypes.FirstOrDefault(x => x.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _db.Booktypes.FirstOrDefault(x => x.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.Booktypes.Remove(obj);
            _db.SaveChanges();
            TempData["delete"] = "Genre has been Removed";
            return RedirectToAction("Index");

        }
    }
}
