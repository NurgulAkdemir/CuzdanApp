using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Interfaces;

namespace UI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {
        private readonly IUserService _userService;

        public UserManagementController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search, string? role, bool? isBanned, string? sortBy = "id", bool sortDesc = false, int page = 1, int pageSize = 10)
        {
            var (items, total) = await _userService.QueryUsersAsync(search, role, isBanned, sortBy, sortDesc, page, pageSize);
            ViewBag.TotalCount = total;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Search = search;
            ViewBag.Role = role;
            ViewBag.IsBanned = isBanned;
            ViewBag.SortBy = sortBy;
            ViewBag.SortDesc = sortDesc;
            ViewBag.Layout = "_LayoutAdmin";
            return View(items);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ban(int id)
        {
            await _userService.SetBanStatusAsync(id, true);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unban(int id)
        {
            await _userService.SetBanStatusAsync(id, false);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteUserAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}


