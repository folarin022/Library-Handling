using LibraryHandling.Data;
using LibraryHandling.Dto.AuthorModel;
using LibraryHandling.Dto.BorrowingModel;
using LibraryHandling.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;



namespace LibraryHandling.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AdminController(
            IAdminService adminService,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _adminService = adminService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        #region Admin Login/Logout

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AdminLogin()
        {
            return View ();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminLogin(LoginDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.IsInRoleAsync(user, "Admin"))
            {
                ModelState.AddModelError("", "Access denied or invalid credentials");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,  // pass email as string
                model.Password,
                false,    
                false         // lockoutOnFailure
            );

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid login attempt");
                return View(model);
            }

            return RedirectToAction("Authors"); // redirect to admin dashboard
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("AdminLogin");
        }

        #endregion

        #region Authors CRUD

        public async Task<IActionResult> Authors(CancellationToken cancellationToken)
        {
            var result = await _adminService.GetAllAuthors(cancellationToken);
            return View(result.Data); // Authors.cshtml
        }

        [HttpGet]
        public IActionResult AddAuthor()
        {
            return View("AddAuthor");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAuthor(AddAuthorDto model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _adminService.AddAuthor(model, cancellationToken);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }

            return RedirectToAction("Authors");
        }


        [HttpGet]
        public async Task<IActionResult> EditAuthor(Guid id, CancellationToken cancellationToken)
        {
            var result = await _adminService.GetAuthorById(id, cancellationToken);
            if (!result.IsSuccess) return NotFound();

            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAuthor(Guid id, AddAuthorDto model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _adminService.UpdateAuthor(id, model, cancellationToken);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }

            return RedirectToAction("Authors");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAuthor(Guid id, CancellationToken cancellationToken)
        {
            await _adminService.DeleteAuthor(id, cancellationToken);
            return RedirectToAction("Authors");
        }

        #endregion

        #region Borrowings CRUD

        public async Task<IActionResult> Borrowings(CancellationToken cancellationToken)
        {
            var result = await _adminService.GetAllBorrowings(cancellationToken);
            return View(result.Data); // Borrowings.cshtml
        }

        [HttpGet]
        public IActionResult AddBorrowing() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBorrowing(AddBorrowingDto model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _adminService.AddBorrowing(model, cancellationToken);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }

            return RedirectToAction("Borrowings");
        }

        [HttpGet]
        public async Task<IActionResult> EditBorrowing(Guid id, CancellationToken cancellationToken)
        {
            var result = await _adminService.GetBorrowingById(id, cancellationToken);
            if (!result.IsSuccess) return NotFound();

            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBorrowing(Guid id, AddBorrowingDto model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _adminService.UpdateBorrowing(id, model, cancellationToken);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }

            return RedirectToAction("Borrowings");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBorrowing(Guid id, CancellationToken cancellationToken)
        {
            await _adminService.DeleteBorrowing(id, cancellationToken);
            return RedirectToAction("Borrowings");
        }

        #endregion
    }
}
