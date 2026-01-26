using LibraryHandling.Data;
using System;

namespace LibraryHandling.Dto.BorrowingModel
{
    public class AddBorrowingDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } 
        public int BookId { get; set; }  

        public DateTime? BorrowDate { get; set; }

        public DateTime? ReturnDate { get; set; }
    }
}
