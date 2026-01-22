using Microsoft.EntityFrameworkCore;
using LibraryHandling.Context;
using LibraryHandling.Data;
using LibraryHandling.Repository.Interface;

namespace LibraryHandling.Repository
{
    public class BorrowingRepository : IBorrowingRepository
    {
        private readonly LibraryManagementDbContext _dbContext;

        public BorrowingRepository(LibraryManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> BorrowBook(Guid bookId, string userId, CancellationToken cancellationToken)
        {
            var book = await _dbContext.Books
                .FirstOrDefaultAsync(b => b.Id == bookId, cancellationToken);

            if (book == null || !book.IsAvailable)
                return false;

            var borrowing = new Borrowing
            {
                BookId = bookId,
                UserId = userId,
                BorrowDate = DateTime.UtcNow
            };

            book.IsAvailable = false;

            await _dbContext.Borrowings.AddAsync(borrowing, cancellationToken);
            _dbContext.Books.Update(book);

            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> ReturnBook(Guid borrowingId, CancellationToken cancellationToken)
        {
            var borrowing = await _dbContext.Borrowings
                .Include(b => b.Book)
                .FirstOrDefaultAsync(b => b.Id == borrowingId, cancellationToken);

            if (borrowing == null || borrowing.ReturnedAt != null)
                return false;

            borrowing.ReturnedAt = DateTime.UtcNow;
            borrowing.Book.IsAvailable = true;

            _dbContext.Borrowings.Update(borrowing);
            _dbContext.Books.Update(borrowing.Book);

            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<List<Borrowing>> GetAllBorrowings(CancellationToken cancellationToken)
        {
            return await _dbContext.Borrowings
                .Include(b => b.Book)
                .Include(b => b.User)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Borrowing>> GetBorrowingsByUser(string userId, CancellationToken cancellationToken)
        {
            return await _dbContext.Borrowings
                .Include(b => b.Book)
                .Where(b => b.UserId == userId)
                .ToListAsync(cancellationToken);
        }
    }
}
