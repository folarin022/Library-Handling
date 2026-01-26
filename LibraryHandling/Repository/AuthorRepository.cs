using LibraryHandling.Context;
using LibraryHandling.Data;
using LibraryHandling.Dto.AuthorModel;
using LibraryHandling.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryHandling.Repository
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly LibraryManagementDbContext _dbContext;

        public AuthorRepository(LibraryManagementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddAuthor(AddAuthorDto authorDto)
        {
            if (authorDto == null) return false;

            var author = new Author
            {
                Id = authorDto.Id == Guid.Empty ? Guid.NewGuid() : authorDto.Id,
                Name = authorDto.Name,
                Bio = authorDto.Bio,
                DOB = authorDto.DOB,
                Books = authorDto.Books ?? new List<Book>()
            };

            await _dbContext.Authors.AddAsync(author);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<List<Author>> GetAllAuthors()
        {
            return await _dbContext.Authors.Include(a => a.Books).ToListAsync();
        }

        public async Task<Author> GetAuthorById(Guid id)
        {
            return await _dbContext.Authors
                                   .Include(a => a.Books)
                                   .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<bool> UpdateAuthor(AddAuthorDto authorDto)
        {
            if (authorDto == null) return false;

            var author = await _dbContext.Authors.FirstOrDefaultAsync(a => a.Id == authorDto.Id);
            if (author == null) return false;

            author.Name = authorDto.Name;
            author.Bio = authorDto.Bio;
            author.DOB = authorDto.DOB;
            // Optional: handle Books update carefully
            // author.Books = authorDto.Books;

            _dbContext.Authors.Update(author);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteAuthor(Guid id)
        {
            var author = await _dbContext.Authors.FirstOrDefaultAsync(a => a.Id == id);
            if (author == null) return false;

            _dbContext.Authors.Remove(author);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }
    }
}
