using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace LibraryHandling.Data
{
    public class Author
    {
        public Author()
        {
            Books = new List<Book>(); // Initialize here
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public DateTime DOB { get; set; }
        [ValidateNever]
        public ICollection<Book> Books { get; set; }
    }

}
