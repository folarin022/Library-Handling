using LibraryHandling.Data;
using LibraryHandling.Dto.BookModel;

namespace LibraryHandling.Repository.Interface
{
    public interface IBookRepository
    {
        Task<bool> AddBook(AddBookDto dto, CancellationToken cancellationToken);
        Task<bool> UpdateBook(AddBookDto dto, CancellationToken cancellationToken);
        Task<bool> DeleteBook(Guid id, CancellationToken cancellationToken);

        Task<Book?> GetBookById(Guid id, CancellationToken cancellationToken);
        Task<List<Book>> GetAllBooks(CancellationToken cancellationToken);
    }
}
