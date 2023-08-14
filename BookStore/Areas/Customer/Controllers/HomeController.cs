using BookStore.Data;
using BookStore.Models;
using BookStore.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using X.PagedList;

namespace BookStore.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(int? page, string searchString, double? minPrice, double? maxPrice)
        {
            var books = _db.Books.Include(x => x.BookTypes).Include(y => y.SpecialTags).ToList();

            if (!string.IsNullOrEmpty(searchString))
            {
                books = books.Where(b =>
                    b.BookName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    b.AuthorName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            //For Price Filter

            if (minPrice != null && maxPrice == null)
            {
                var maxPriceFilter = 100000;
                books = books.Where(b => b.Price >= minPrice && b.Price <= maxPriceFilter).ToList();
            }
            else if (minPrice == null && maxPrice != null)
            {
                var minPriceFilter = 0;
                books = books.Where(b => b.Price >= minPriceFilter && b.Price <= maxPrice).ToList();
            }
            else if (minPrice != null && maxPrice != null)
            {
                books = books.Where(b => b.Price >= minPrice && b.Price <= maxPrice).ToList();
            }

            var obj = books.ToPagedList(page ?? 1, 12);
            return View(obj);
        }


        //Get
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var obj = _db.Books.Include(x => x.BookTypes).Include(y => y.SpecialTags).FirstOrDefault(x => x.Id == id);
            return View(obj);
        }


        [HttpPost]
        [ActionName("Details")]
        public IActionResult BookDetails(int? id)
        {
            List<Books> book = new List<Books>();
            if (id == null)
            {
                return NotFound();
            }
            var obj = _db.Books.Include(x => x.BookTypes).Include(y => y.SpecialTags).FirstOrDefault(x => x.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            book = HttpContext.Session.Get<List<Books>>("book");
            if (book == null)
            {
                book = new List<Books>();
            }
            book.Add(obj);
            HttpContext.Session.Set("book", book);
            return RedirectToAction("Details");
        }


        //Get
        [ActionName("Remove")]
        public IActionResult RemoveCart(int? id)
        {
            List<Books> book = HttpContext.Session.Get<List<Books>>("book");
            if (book != null)
            {
                var product = book.FirstOrDefault(c => c.Id == id);
                if (product != null)
                {
                    book.Remove(product);
                    HttpContext.Session.Set("book", book);
                }
            }
            return RedirectToAction("Cart");
        }


        [HttpPost]
        public IActionResult Remove(int? id)
        {
            List<Books> book = HttpContext.Session.Get<List<Books>>("book");
            if (book != null)
            {
                var product = book.FirstOrDefault(c => c.Id == id);
                if (product != null)
                {
                    book.Remove(product);
                    HttpContext.Session.Set("book", book);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        //Get
        public IActionResult Cart()
        {
            List<Books> book = HttpContext.Session.Get<List<Books>>("book");
            if (book == null)
            {
                book = new List<Books>();
            }
            return View(book);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}