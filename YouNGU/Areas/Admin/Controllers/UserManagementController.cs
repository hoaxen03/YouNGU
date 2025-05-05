using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using YouNGU.Areas.Admin.Models;
using YouNGU.Models.Entities;

namespace YouNGU.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserManagementController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users;
            return View(users);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles;

            var model = new EditUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                UserRoles = userRoles,
                AllRoles = allRoles
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model, string[] selectedRoles)
        {
            if (!ModelState.IsValid)
            {
                model.AllRoles = _roleManager.Roles;
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.FullName = model.FullName;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                model.AllRoles = _roleManager.Roles;
                return View(model);
            }

            // Cập nhật vai trò
            var userRoles = await _userManager.GetRolesAsync(user);
            selectedRoles = selectedRoles ?? new string[] { };

            // Xóa vai trò hiện tại
            result = await _userManager.RemoveFromRolesAsync(user, userRoles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Không thể xóa vai trò hiện tại");
                model.AllRoles = _roleManager.Roles;
                return View(model);
            }

            // Thêm vai trò mới
            result = await _userManager.AddToRolesAsync(user, selectedRoles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Không thể thêm vai trò mới");
                model.AllRoles = _roleManager.Roles;
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Xóa người dùng không thành công");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
