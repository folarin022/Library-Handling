using LibraryHandling.Data;
using LibraryHandling.Dto.BorrowingModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryHandling.Repository.Interface
{
    public interface IBorrowingRepository
    {
        Task<bool> AddBorrowing(AddBorrowingDto borrowingDto);
        Task<List<Borrowing>> GetAllBorrowings();
        Task<Borrowing> GetBorrowingById(Guid id);
        Task<bool> UpdateBorrowing(AddBorrowingDto borrowingDto);
        Task<bool> DeleteBorrowing(Guid id);
    }
}
