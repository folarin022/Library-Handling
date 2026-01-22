using LibraryHandling.Data;

namespace LibraryHandling.Dto.AuthorModel
{
    public class AddAuthorDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public DateTime DOB { get; set; }

        public ICollection<Book> Books { get; set; }
    }
}
