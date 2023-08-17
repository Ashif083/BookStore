using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BookStore.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = "Admin")]
    public class UserRolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
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
                return RedirectToAction("ViewAllRoles");
            }
            return View(obj);
        }

    }
}
