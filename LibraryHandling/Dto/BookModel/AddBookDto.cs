namespace LibraryHandling.Dto.BookModel
{
    public class AddBookDto
    {
        public Guid Id { get; set; }
        public string BookTitle { get; set; }
        public string Description { get; set; }
    }
}
