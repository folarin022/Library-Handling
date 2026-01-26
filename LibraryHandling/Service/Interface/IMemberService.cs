using LibraryHandling.Data;
using LibraryHandling.Dto;
using LibraryHandling.Dto.BorrowingModel;

namespace LibraryHandling.Service.Interface
{
    public interface IMemberService
    {
        Task<BaseResponse<Borrowing>> BorrowBook(AddBorrowingDto request, CancellationToken cancellationToken);
        Task<BaseResponse<List<Borrowing>>> GetMyBorrowings(string userId, CancellationToken cancellationToken);
        Task<BaseResponse<bool>> ReturnBook(Guid borrowingId, CancellationToken cancellationToken);
    }
}
