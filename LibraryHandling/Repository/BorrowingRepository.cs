using LibraryHandling.Context;
using LibraryHandling.Data;
using LibraryHandling.Dto.BorrowingModel;
using LibraryHandling.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryHandling.Repository
{
    public class BorrowingRepository : IBorrowingRepository
    {
        private readonly LibraryManagementDbContext _dbContext;

        public BorrowingRepository(LibraryManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddBorrowing(AddBorrowingDto borrowingDto)
        {
            if (borrowingDto == null) return false;

            var borrowing = new Borrowing
            {
                Id = Guid.NewGuid(),
                UserId = borrowingDto.UserId,
                BookId = borrowingDto.BookId,
                BorrowDate = borrowingDto.BorrowDate ?? DateTime.Now,
                ReturnDate = borrowingDto.ReturnDate
            };

            await _dbContext.Borrowings.AddAsync(borrowing);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<List<Borrowing>> GetAllBorrowings()
        {
            return await _dbContext.Borrowings
                                   .Include(b => b.Book)
                                   .Include(b => b.User)
                                   .ToListAsync();
        }

        public async Task<Borrowing> GetBorrowingById(Guid id)
        {
            return await _dbContext.Borrowings
                                   .Include(b => b.Book)
                                   .Include(b => b.User)
                                   .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<bool> UpdateBorrowing(AddBorrowingDto borrowingDto)
        {
            if (borrowingDto == null) return false;

            var borrowing = await _dbContext.Borrowings.FirstOrDefaultAsync(b => b.Id == borrowingDto.Id);
            if (borrowing == null) return false;

            borrowing.UserId = borrowingDto.UserId;
            borrowing.BookId = borrowingDto.BookId;
            borrowing.BorrowDate = borrowingDto.BorrowDate ?? borrowing.BorrowDate;
            borrowing.ReturnDate = borrowingDto.ReturnDate;

            _dbContext.Borrowings.Update(borrowing);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteBorrowing(Guid id)
        {
            var borrowing = await _dbContext.Borrowings.FirstOrDefaultAsync(b => b.Id == id);
            if (borrowing == null) return false;

            _dbContext.Borrowings.Remove(borrowing);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }
    }
}
