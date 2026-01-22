namespace LibraryHandling.Data
{
    public class Book
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Year { get; set; }
        public bool IsAvailable { get; set; } = true;

        public Guid AuthorId { get; set; }
        public Author Author { get; set; }
    }

}
