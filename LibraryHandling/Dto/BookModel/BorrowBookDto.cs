using System.ComponentModel.DataAnnotations;

namespace LibraryHandling.Dto.BookModel
{
    public class BorrowBookDto
    {
        public Guid BookId { get; set; }
        [Required(ErrorMessage = "Please select a book")]
        public string SelectedBookTitle { get; set; }

        public DateTime? BorrowDate { get; set; }
    }
}
