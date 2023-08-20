using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TagsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public TagsController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            var obj = _db.SpecialTags.ToList();
            return View(obj); ;
        }
        //Get
        public IActionResult Create()
        {
            return View(); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SpecialTags tag)
        {
            if (ModelState.IsValid)
            {
                var checkTag = _db.SpecialTags.FirstOrDefault(x => x.TagName == tag.TagName);
                if (checkTag != null)
                {
                    ViewBag.message = "This Tag is already existed";
                    return View(tag);
                }
                _db.SpecialTags.Add(tag);
                _db.SaveChanges();
                TempData["save"] = "New Tag has been Created";
                return RedirectToAction("Index");
            }
            return View(tag);
        }

        //Get
        public IActionResult Edit(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }
            var obj = _db.SpecialTags.FirstOrDefault(x => x.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(SpecialTags tag)
        {
            if (ModelState.IsValid)
            {
                var checkTag = _db.SpecialTags.FirstOrDefault(x => x.TagName == tag.TagName);
                if (checkTag != null)
                {
                    ViewBag.message = "This Tag is already existed";
                    return View(tag);
                }
                _db.SpecialTags.Update(tag);
                _db.SaveChanges();
                TempData["edit"] = "Tag Name has been Updated";
                return RedirectToAction("Index");
            }
            return View(tag);
        }
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var obj = _db.SpecialTags.FirstOrDefault(x => x.Id == id);
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
            var obj = _db.SpecialTags.FirstOrDefault(x => x.Id == id);
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
            var obj = _db.SpecialTags.FirstOrDefault(x => x.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.SpecialTags.Remove(obj);
            _db.SaveChanges();
            TempData["delete"] = "Tag has been Removed";
            return RedirectToAction("Index");

        }

        public IActionResult Adminview()
        {
            return PartialView("_AdminPanel");
        }



    }
}

