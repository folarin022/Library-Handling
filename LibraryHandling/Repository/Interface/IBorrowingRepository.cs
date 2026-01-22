using LibraryHandling.Data;

namespace LibraryHandling.Repository.Interface
{
    public interface IBorrowingRepository
    {
        Task<bool> BorrowBook(Guid bookId, string userId, CancellationToken cancellationToken);
        Task<bool> ReturnBook(Guid borrowingId, CancellationToken cancellationToken);

        Task<List<Borrowing>> GetAllBorrowings(CancellationToken cancellationToken);
        Task<List<Borrowing>> GetBorrowingsByUser(string userId, CancellationToken cancellationToken);
    }
}
