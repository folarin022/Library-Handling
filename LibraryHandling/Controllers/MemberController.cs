using LibraryHandling.Context;
using LibraryHandling.Data;
using LibraryHandling.Dto.BookModel;
using LibraryHandling.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryHandling.Controllers
{
    [Authorize(Roles = "Member")]
    public class MemberController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly LibraryManagementDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public MemberController(
            IMemberService memberService,
            LibraryManagementDbContext dbContext,
            UserManager<ApplicationUser> userManager)
        {
            _memberService = memberService;
            _dbContext = dbContext;
            _userManager = userManager;
        }

        #region Borrowing Operations

        [HttpGet]
        public async Task<IActionResult> MyBorrowings(CancellationToken cancellationToken)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var result = await _memberService.GetMyBorrowings(user.Id, cancellationToken);

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Message;
                return View(new List<Borrowing>());
            }

            return View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableBooks(string search = "", CancellationToken cancellationToken = default)
        {
            var borrowedBookIds = await _dbContext.Borrowings
                .Where(b => b.ReturnDate == null)
                .Select(b => b.BookId)
                .ToListAsync(cancellationToken);

            var books = await _dbContext.Books
                .Include(b => b.Author)
                .Where(b => !borrowedBookIds.Contains(b.Id))
                .Where(b => string.IsNullOrWhiteSpace(search) ||
            EF.Functions.Like(b.Title, $"%{search}%"))

                .Select(b => new
                {
                    b.Id,
                    b.Title,
                    AuthorName = b.Author != null ? b.Author.Name : "Unknown Author",
                })
                .Take(20)
                .ToListAsync(cancellationToken);

            return Json(books);
        }

        [HttpGet]
        public async Task<IActionResult> BorrowBook(CancellationToken cancellationToken)
        {
            var borrowedBookIds = await _dbContext.Borrowings
                .Where(b => b.ReturnDate == null)
                .Select(b => b.BookId)
                .ToListAsync(cancellationToken);

            var books = await _dbContext.Books
                .Include(b => b.Author)
                .Where(b => !borrowedBookIds.Contains(b.Id))
                .Select(b => new
                {
                    b.Id,
                    b.Title,
                    AuthorName = b.Author.Name,
                })
                .ToListAsync(cancellationToken);

            ViewBag.Books = books;
            return View(new BorrowBookDto());
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BorrowBook(BorrowBookDto model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var activeBorrowings = await _dbContext.Borrowings
                .CountAsync(b => b.UserId == user.Id && b.ReturnDate == null, cancellationToken);

            if (activeBorrowings >= 5)
            {
                ModelState.AddModelError("", "You have reached the maximum limit of 5 active borrowings.");
                return View(model);
            }

            var book = await _dbContext.Books
    .FirstOrDefaultAsync(b => b.Id == model.BookId, cancellationToken);

            if (book == null)
            {
                ModelState.AddModelError("SelectedBookTitle", "Book not found. Please select a valid book.");
                return View(model);
            }

            var isBorrowed = await _dbContext.Borrowings
                .AnyAsync(b => b.BookId == book.Id && b.ReturnDate == null, cancellationToken);

            if (isBorrowed)
            {
                ModelState.AddModelError("SelectedBookTitle", "This book is currently borrowed.");
                return View(model);
            }

            var borrowing = new Borrowing
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                BookId = book.Id,
                BorrowDate = model.BorrowDate ?? DateTime.Now
            };

            await _dbContext.Borrowings.AddAsync(borrowing, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            TempData["Success"] = $"You have successfully borrowed '{book.Title}'.";
            return RedirectToAction("MyBorrowings");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnBook(Guid id, CancellationToken cancellationToken)
        {
            var result = await _memberService.ReturnBook(id, cancellationToken);

            TempData[result.IsSuccess ? "Success" : "Error"] =
                result.IsSuccess ? "Book returned successfully." : result.Message;

            return RedirectToAction("MyBorrowings");
        }

        #endregion
    }
}
