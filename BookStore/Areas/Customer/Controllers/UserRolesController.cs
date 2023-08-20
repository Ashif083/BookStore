using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookStore.Models;
using BookStore.Data;
using BookStore.Areas.Customer.Views;
using Microsoft.AspNetCore.Mvc.Rendering;
using BookStore.Areas.Customer.Model;

namespace BookStore.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = "Admin")]
    public class UserRolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        

        public UserRolesController(RoleManager<IdentityRole> roleManager,ApplicationDbContext db,UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _db = db;
            _userManager = userManager;
        }

        //List All Roles
        [HttpGet]
        public IActionResult ViewAllRoles()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(IdentityRole role)
        {
            if (!_roleManager.RoleExistsAsync(role.Name).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(role.Name)).GetAwaiter().GetResult();
            }
            TempData["save"] = "New Role has been Created";
            return RedirectToAction("ViewAllRoles");
        }

        //Get
        public async  Task<IActionResult> Edit(string id)
        {
            var obj= await _roleManager.FindByIdAsync(id);
            if(obj==null)
            {
                return NotFound();
            }
            return View(obj);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, IdentityRole role)
        {
            var obj= await _roleManager.FindByIdAsync(id);
            if(obj==null)
            {
                return NotFound();
            }
            obj.Name = role.Name;
            var result= await _roleManager.UpdateAsync(obj);
            if(result.Succeeded)
            {
                TempData["edit"] = "Role has been Updated";
                return RedirectToAction("ViewAllRoles");
            }
            return View(role);
        }

        //Get
        public async Task<IActionResult> Delete(string id)
        {
            var obj = await _roleManager.FindByIdAsync(id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }
        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePost(string id)
        {
            var obj = await _roleManager.FindByIdAsync(id);
            if (obj == null)
            {
                return NotFound();
            }
            var result = await _roleManager.DeleteAsync(obj);
            if (result.Succeeded)
            {
                TempData["delete"] = "Role has been Removed";
                return RedirectToAction("ViewAllRoles");
            }
            return View(obj);
        }
       
        ///For Users--------------------------------------->

        public IActionResult ShowUsers()
        {
            var obj = _db.ApplicationUsers.ToList();
            return View(obj);
        }

        //public IActionResult AddUser()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> AddUser(ApplicationUsers user)
        //{
        //    if(ModelState.IsValid)
        //    {
        //        var result = await _userManager.CreateAsync(user);
        //        if(result.Succeeded)
        //        {
        //            return RedirectToAction("ShowUsers");
        //        }
        //    }
        //    return View();
        //}

        public IActionResult EditUser(string id)
        {
            var user= _db.ApplicationUsers.FirstOrDefault(x => x.Id == id);
            if(user==null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(ApplicationUsers user)
        {
            var userinfo = _db.ApplicationUsers.FirstOrDefault(x => x.Id == user.Id);
            if(userinfo==null)
            {
                return NotFound();
            }
            userinfo.FirstName = user.FirstName;
            userinfo.LastName=user.LastName;
            userinfo.Address = user.Address;
            userinfo.PhoneNumber = user.PhoneNumber;
            var result =await _userManager.UpdateAsync(userinfo);
            if(result.Succeeded)
            {
                return RedirectToAction("ShowUsers");
            }
            return View(userinfo);
        }

        public IActionResult DeleteUser(string id)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(x => x.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(ApplicationUsers user)
        {
            var userinfo = _db.ApplicationUsers.FirstOrDefault(x => x.Id == user.Id);
            if (userinfo == null)
            {
                return NotFound();
            }
            _db.ApplicationUsers.Remove(userinfo);
            int rowEffected = _db.SaveChanges();
            if (rowEffected>0)
            {
                return RedirectToAction("ShowUsers");
            }
            return View(userinfo);
        }


        //Show User Information & Role
        public IActionResult UserInformationwithRole()
        {
            var result = from ur in _db.UserRoles
                         join r in _db.Roles on ur.RoleId equals r.Id
                         join a in _db.ApplicationUsers on ur.UserId equals a.Id
                         select new UserRoleMapping()
                         {
                             UserId = ur.UserId,
                             RoleId = ur.RoleId,
                             UserName=a.UserName,
                             RoleName=r.Name
                         };
            ViewBag.UserRoles=result;

            return View();
        }

        public IActionResult Assign(string userId, string roleId)
        {
            AssignRoleViewModel model = new AssignRoleViewModel
            {
                UserId = userId,
                RoleId = roleId
            };

            ViewData["UserId"] = new SelectList(_db.ApplicationUsers.ToList(), "Id", "UserName");
            ViewData["RoleId"] = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");
            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> Assign(AssignRoleViewModel roleUser)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(x => x.Id == roleUser.UserId);

            var existingRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, existingRoles);

            if (!removeResult.Succeeded)
            {
                return NotFound();
            }
            var addResult = await _userManager.AddToRoleAsync(user, roleUser.RoleId);

            if (addResult.Succeeded)
            {
                TempData["save"] = "User Role Assigned Successfully";
                return RedirectToAction("UserInformationwithRole");
            }

            return View();
        }

    }
}
