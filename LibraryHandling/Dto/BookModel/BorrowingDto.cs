namespace LibraryHandling.Dto.BookModel
{
    public class BorrowingDto
    {
        public Guid Id { get; set; }
        public Guid MemberId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public Guid BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnedDate { get; set; }
        public string? Notes { get; set; }

        // Calculated properties (optional)
        public bool IsOverdue => !ReturnedDate.HasValue && DueDate < DateTime.Now;
        public bool IsActive => !ReturnedDate.HasValue && DueDate >= DateTime.Now;
        public int DaysOverdue => IsOverdue ? (DateTime.Now - DueDate).Days : 0;
    }
}