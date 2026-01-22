using Microsoft.EntityFrameworkCore;
using LibraryHandling.Context;
using LibraryHandling.Data;
using LibraryHandling.Dto.BookModel;
using LibraryHandling.Repository.Interface;

namespace LibraryHandling.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly LibraryManagementDbContext _dbContext;

        public BookRepository(LibraryManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddBook(AddBookDto dto, CancellationToken cancellationToken)
        {
            var book = new Book
            {
                Title = dto.BookTitle,
                Description = dto.Description,
                IsAvailable = true
            };

            await _dbContext.Books.AddAsync(book, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteBook(Guid id, CancellationToken cancellationToken)
        {
            var book = await _dbContext.Books
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

            if (book == null)
                return false;

            _dbContext.Books.Remove(book);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<List<Book>> GetAllBooks(CancellationToken cancellationToken)
        {
            return await _dbContext.Books
                .Include(b => b.Author)
                .ToListAsync(cancellationToken);
        }

        public async Task<Book?> GetBookById(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Books
                .Include(b => b.Author)
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        }

        public async Task<List<Book>> GetAvailableBooks(CancellationToken cancellationToken)
        {
            return await _dbContext.Books
                .Where(b => b.IsAvailable)
                .Include(b => b.Author)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> UpdateBook(AddBookDto dto, CancellationToken cancellationToken)
        {
            var book = await _dbContext.Books
                .FirstOrDefaultAsync(b => b.Id == dto.Id, cancellationToken);

            if (book == null)
                return false;

            book.Title = dto.BookTitle;
            book.Description = dto.Description;

            _dbContext.Books.Update(book);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
