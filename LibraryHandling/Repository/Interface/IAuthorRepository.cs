using LibraryHandling.Data;
using LibraryHandling.Dto.AuthorModel;

namespace LibraryHandling.Repository.Interface
{
    public interface IAuthorRepository
    {
        Task<bool> AddAuthor(AddAuthorDto authorDto);
        Task<List<Author>> GetAllAuthors();
        Task<Author> GetAuthorById(Guid id);
        Task<bool> UpdateAuthor(AddAuthorDto authorDto);
        Task<bool> DeleteAuthor(Guid id);
    }
}
