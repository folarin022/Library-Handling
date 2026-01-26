using LibraryHandling.Data;
using LibraryHandling.Dto;
using LibraryHandling.Dto.AuthorModel;
using LibraryHandling.Dto.BorrowingModel;

namespace LibraryHandling.Service.Interface
{
    public interface IAdminService
    {
        Task<BaseResponse<Author>> AddAuthor(AddAuthorDto request, CancellationToken cancellationToken);
        Task<BaseResponse<List<Author>>> GetAllAuthors(CancellationToken cancellationToken);
        Task<BaseResponse<Author>> GetAuthorById(Guid id, CancellationToken cancellationToken);
        Task<BaseResponse<bool>> UpdateAuthor(Guid id, AddAuthorDto request, CancellationToken cancellationToken);
        Task<BaseResponse<bool>> DeleteAuthor(Guid id, CancellationToken cancellationToken);

        Task<BaseResponse<Borrowing>> AddBorrowing(AddBorrowingDto request, CancellationToken cancellationToken);
        Task<BaseResponse<List<Borrowing>>> GetAllBorrowings(CancellationToken cancellationToken);
        Task<BaseResponse<Borrowing>> GetBorrowingById(Guid id, CancellationToken cancellationToken);
        Task<BaseResponse<bool>> UpdateBorrowing(Guid id, AddBorrowingDto request, CancellationToken cancellationToken);
        Task<BaseResponse<bool>> DeleteBorrowing(Guid id, CancellationToken cancellationToken);
    }
}
