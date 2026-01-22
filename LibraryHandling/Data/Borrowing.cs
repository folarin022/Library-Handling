namespace LibraryHandling.Data
{
    public class Borrowing
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } 
        public ApplicationUser User { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }

        public DateTime BorrowDate { get; set; } = DateTime.Now;
        public DateTime? ReturnDate { get; set; }
    }

}
