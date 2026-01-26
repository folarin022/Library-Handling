using LibraryHandling.Context;
using LibraryHandling.Data;
using LibraryHandling.Dto;
using LibraryHandling.Dto.AuthorModel;
using LibraryHandling.Dto.BorrowingModel;
using LibraryHandling.Repository.Interface;
using LibraryHandling.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace LibraryHandling.Service
{
    public class AdminService : IAdminService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IBorrowingRepository _borrowingRepository;
        private readonly LibraryManagementDbContext _dbContext;

        public AdminService(
            IAuthorRepository authorRepository,
            IBorrowingRepository borrowingRepository,
            LibraryManagementDbContext dbContext)
        {
            _authorRepository = authorRepository;
            _borrowingRepository = borrowingRepository;
            _dbContext = dbContext;
        }

        #region Author Operations

        public async Task<BaseResponse<Author>> AddAuthor(AddAuthorDto request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<Author>();

            try
            {
                var author = new Author
                {
                    Id = request.Id == Guid.Empty ? Guid.NewGuid() : request.Id,
                    Name = request.Name,
                    Bio = request.Bio,
                    DOB = request.DOB,
                    Books = request.Books ?? new List<Book>()
                };

                await _dbContext.Authors.AddAsync(author, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                response.IsSuccess = true;
                response.Data = author;
                response.Message = "Author added successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<List<Author>>> GetAllAuthors(CancellationToken cancellationToken)
        {
            var response = new BaseResponse<List<Author>>();

            try
            {
                var authors = await _dbContext.Authors.Include(a => a.Books).ToListAsync(cancellationToken);
                response.IsSuccess = true;
                response.Data = authors;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<Author>> GetAuthorById(Guid id, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<Author>();

            try
            {
                var author = await _dbContext.Authors.Include(a => a.Books)
                    .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

                if (author == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Author not found";
                }
                else
                {
                    response.IsSuccess = true;
                    response.Data = author;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> UpdateAuthor(Guid id, AddAuthorDto request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var author = await _dbContext.Authors.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
                if (author == null)
                {
                    response.IsSuccess = false;
                    response.Data = false;
                    response.Message = "Author not found";
                    return response;
                }

                author.Name = request.Name;
                author.Bio = request.Bio;
                author.DOB = request.DOB;
                // Optional: handle Books updates if needed

                await _dbContext.SaveChangesAsync(cancellationToken);

                response.IsSuccess = true;
                response.Data = true;
                response.Message = "Author updated successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Data = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> DeleteAuthor(Guid id, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var author = await _dbContext.Authors.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
                if (author == null)
                {
                    response.IsSuccess = false;
                    response.Data = false;
                    response.Message = "Author not found";
                    return response;
                }

                _dbContext.Authors.Remove(author);
                await _dbContext.SaveChangesAsync(cancellationToken);

                response.IsSuccess = true;
                response.Data = true;
                response.Message = "Author deleted successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Data = false;
                response.Message = ex.Message;
            }

            return response;
        }

        #endregion

        #region Borrowing Operations

        public async Task<BaseResponse<Borrowing>> AddBorrowing(AddBorrowingDto request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<Borrowing>();

            try
            {
                var existing = await _dbContext.Borrowings
                    .FirstOrDefaultAsync(b => b.BookId == request.BookId && b.ReturnDate == null, cancellationToken);

                if (existing != null)
                {
                    response.IsSuccess = false;
                    response.Message = "Book is already borrowed";
                    return response;
                }

                var borrowing = new Borrowing
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    BookId = request.BookId,
                    BorrowDate = request.BorrowDate ?? DateTime.Now,
                    ReturnDate = request.ReturnDate
                };

                await _dbContext.Borrowings.AddAsync(borrowing, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                response.IsSuccess = true;
                response.Data = borrowing;
                response.Message = "Borrowing added successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<List<Borrowing>>> GetAllBorrowings(CancellationToken cancellationToken)
        {
            var response = new BaseResponse<List<Borrowing>>();

            try
            {
                var borrowings = await _dbContext.Borrowings
                    .Include(b => b.Book)
                    .Include(b => b.User)
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

        public async Task<BaseResponse<Borrowing>> GetBorrowingById(Guid id, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<Borrowing>();

            try
            {
                var borrowing = await _dbContext.Borrowings
                    .Include(b => b.Book)
                    .Include(b => b.User)
                    .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

                if (borrowing == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Borrowing not found";
                }
                else
                {
                    response.IsSuccess = true;
                    response.Data = borrowing;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> UpdateBorrowing(Guid id, AddBorrowingDto request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var borrowing = await _dbContext.Borrowings.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
                if (borrowing == null)
                {
                    response.IsSuccess = false;
                    response.Data = false;
                    response.Message = "Borrowing not found";
                    return response;
                }

                borrowing.UserId = request.UserId;
                borrowing.BookId = request.BookId;
                borrowing.BorrowDate = request.BorrowDate ?? borrowing.BorrowDate;
                borrowing.ReturnDate = request.ReturnDate;

                await _dbContext.SaveChangesAsync(cancellationToken);

                response.IsSuccess = true;
                response.Data = true;
                response.Message = "Borrowing updated successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Data = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> DeleteBorrowing(Guid id, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<bool>();

            try
            {
                var borrowing = await _dbContext.Borrowings.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
                if (borrowing == null)
                {
                    response.IsSuccess = false;
                    response.Data = false;
                    response.Message = "Borrowing not found";
                    return response;
                }

                _dbContext.Borrowings.Remove(borrowing);
                await _dbContext.SaveChangesAsync(cancellationToken);

                response.IsSuccess = true;
                response.Data = true;
                response.Message = "Borrowing deleted successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Data = false;
                response.Message = ex.Message;
            }

            return response;
        }

        #endregion
    }
}
