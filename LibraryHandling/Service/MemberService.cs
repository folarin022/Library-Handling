using LibraryHandling.Context;
using LibraryHandling.Data;
using LibraryHandling.Dto;
using LibraryHandling.Dto.BorrowingModel;
using LibraryHandling.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace LibraryHandling.Service
{
    public class MemberService : IMemberService
    {
        private readonly LibraryManagementDbContext _dbContext;

        public MemberService(LibraryManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Borrow a book
        public async Task<BaseResponse<Borrowing>> BorrowBook(AddBorrowingDto request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<Borrowing>();

            try
            {
                // Check if the book is already borrowed and not returned
                var existing = await _dbContext.Borrowings
                    .FirstOrDefaultAsync(b => b.BookId == request.BookId && b.ReturnDate == null, cancellationToken);

                if (existing != null)
                {
                    response.IsSuccess = false;
                    response.Message = "Book is already borrowed by another member.";
                    return response;
                }

                var borrowing = new Borrowing
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    BookId = request.BookId,
                    BorrowDate = request.BorrowDate ?? DateTime.Now
                };

                await _dbContext.Borrowings.AddAsync(borrowing, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                response.IsSuccess = true;
                response.Data = borrowing;
                response.Message = "Book borrowed successfully.";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        // Get all borrowings for a member
        public async Task<BaseResponse<List<Borrowing>>> GetMyBorrowings(string userId, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<List<Borrowing>>();

            try
            {
                var borrowings = await _dbContext.Borrowings
                    .Include(b => b.Book)
                    .Where(b => b.UserId == userId)
                    .ToListAsync(cancellationToken);

                response.IsSuccess = true;
                response.Data = borrowings;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> ReturnBook(Guid borrowingId, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<bool>();
            
            try
            {
                var borrowing = await _dbContext.Borrowings.FirstOrDefaultAsync(b => b.Id == borrowingId, cancellationToken);
                if (borrowing == null)
                {
                    response.IsSuccess = false;
                    response.Data = false;
                    response.Message = "Borrowing record not found.";
                    return response;
                }

                if (borrowing.ReturnDate != null)
                {
                    response.IsSuccess = false;
                    response.Data = false;
                    response.Message = "Book is already returned.";
                    return response;
                }

                borrowing.ReturnDate = DateTime.Now;
                await _dbContext.SaveChangesAsync(cancellationToken);

                response.IsSuccess = true;
                response.Data = true;
                response.Message = "Book returned successfully.";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Data = false;
                response.Message = ex.Message;
            }

            return response;
        }
    }
}
