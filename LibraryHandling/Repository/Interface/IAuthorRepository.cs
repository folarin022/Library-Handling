using LibraryHandling.Data;

namespace LibraryHandling.Repository.Interface
{
    public interface IAuthorRepository
    {
        Task<bool> AddAuthor(Author author);
        Task<List<Author>> GetAllAuthors();
    }
}
