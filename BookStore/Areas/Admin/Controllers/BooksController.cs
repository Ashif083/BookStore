using Microsoft.AspNetCore.Mvc;
using BookStore.Data;
using Microsoft.EntityFrameworkCore;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles="Admin")]
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _he;
        public BooksController(ApplicationDbContext db, IWebHostEnvironment he)
        {
            _db = db;
            _he = he;
        }

        public IActionResult Index()
        {
            var obj = _db.Books.Include(x => x.BookTypes).Include(y => y.SpecialTags).ToList();
            return View(obj);
        }
        [HttpPost]
        public IActionResult Index(double? lowAmount, double? highAmount) //For Price Range
        {;
            var obj = _db.Books.Include(c => c.BookTypes).Include(c => c.SpecialTags).Where(c => c.Price >= lowAmount && c.Price <= highAmount).ToList();
            if (lowAmount != null && highAmount == null)
            {
                var lastAmount = 10000000;
                var obj2 = _db.Books.Include(c => c.BookTypes).Include(c => c.SpecialTags).Where(c => c.Price >= lowAmount && c.Price <= lastAmount).ToList();
                return View(obj2);
            }
            else if (lowAmount == null && highAmount != null)
            {
                var firstAmount = 0;
                var obj3 = _db.Books.Include(c => c.BookTypes).Include(c => c.SpecialTags).Where(c => c.Price >= firstAmount && c.Price <= highAmount).ToList();
                return View(obj3);
            }
            else if (lowAmount == null && highAmount == null)
            {
                var obj4 = _db.Books.Include(c => c.BookTypes).Include(c => c.SpecialTags).ToList();
                return View(obj4);
            }
            return View(obj);
        }
        //Get
        public IActionResult Create()
        {
            ViewData["bookTypeId"] = new SelectList(_db.Booktypes.ToList(), "Id", "BookType");
            ViewData["specialTagId"] = new SelectList(_db.SpecialTags.ToList(), "Id", "TagName");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Books books, IFormFile image)
        {
            ViewData["bookTypeId"] = new SelectList(_db.Booktypes.ToList(), "Id", "BookType");
            ViewData["specialTagId"] = new SelectList(_db.SpecialTags.ToList(), "Id", "TagName");
            if (ModelState.IsValid)
            {
                var checkDuplicate = _db.Books.FirstOrDefault(x => x.BookName == books.BookName);
                if(checkDuplicate != null)
                {
                    ViewBag.msg = "This Book is already Existed";
                    ViewData["bookTypeId"] = new SelectList(_db.Booktypes.ToList(), "Id", "BookType");
                    ViewData["specialTagId"] = new SelectList(_db.SpecialTags.ToList(), "Id", "TagName");
                    return View(books);
                }
                if (image != null)
                {
                    var name = Path.Combine(_he.WebRootPath + "/Images", Path.GetFileName(image.FileName));
                    await image.CopyToAsync(new FileStream(name, FileMode.Create));
                    books.Image = "Images/" + image.FileName;
                }
                if (image == null)
                {
                    books.Image = "Images/no-image.jpg";
                }
                _db.Books.Add(books);
                TempData["save"] = "New Book has been Added";
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(books);
        }

        //Get
        public IActionResult Edit(int? id)
        {
            ViewData["bookTypeId"] = new SelectList(_db.Booktypes.ToList(), "Id", "BookType");
            ViewData["specialTagId"] = new SelectList(_db.SpecialTags.ToList(), "Id", "TagName");
            var obj = _db.Books.Include(x => x.BookTypes).Include(x => x.SpecialTags).FirstOrDefault(x => x.Id == id);
            return View(obj);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Books books, IFormFile? image)
        {
            if (ModelState.IsValid)
            {
                ViewData["specialTagId"] = new SelectList(_db.SpecialTags.ToList(), "Id", "TagName");
                ViewData["bookTypeId"] = new SelectList(_db.Booktypes.ToList(), "Id", "BookType");

                if (image != null)
                {
                    var name = Path.Combine(_he.WebRootPath + "/Images", Path.GetFileName(image.FileName));
                    await image.CopyToAsync(new FileStream(name, FileMode.Create));
                    books.Image = "Images/" + image.FileName;
                }

                if (image == null)
                {
                    books.Image = "Images/no-image.JPG";
                }
                _db.Books.Update(books);
                TempData["edit"] = "Edited Successfully";
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(books);
        }
        //Get
        public ActionResult Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            var obj = _db.Books.Include(x => x.BookTypes).Include(x => x.SpecialTags).FirstOrDefault(x => x.Id == id);
            ViewData["specialTagId"] = new SelectList(_db.SpecialTags.ToList(), "Id", "TagName");
            ViewData["bookTypeId"] = new SelectList(_db.Booktypes.ToList(), "Id", "BookType");
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        //Get
        public IActionResult Delete(int? id)
        {
            ViewData["specialTagId"] = new SelectList(_db.SpecialTags.ToList(), "Id", "TagName");
            ViewData["bookTypeId"] = new SelectList(_db.Booktypes.ToList(), "Id", "BookType");
            var obj = _db.Books.Include(x => x.BookTypes).Include(x => x.SpecialTags).FirstOrDefault(x => x.Id == id);
            return View(obj);
        }
        [HttpPost]
        [ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            var obj = _db.Books.FirstOrDefault(x => x.Id == id);
           _db.Books.Remove(obj);
            _db.SaveChanges();
            TempData["delete"] = "Deleted Successfully";
            return RedirectToAction("Index");

        }



    }
}
