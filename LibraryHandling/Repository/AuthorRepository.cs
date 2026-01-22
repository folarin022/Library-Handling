using LibraryHandling.Context;
using LibraryHandling.Data;
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

        public async Task<bool> AddAuthor(Author author)
        {
            if (author == null) return false;

            if (author.Id == Guid.Empty)
                author.Id = Guid.NewGuid();

            await _dbContext.Authors.AddAsync(author);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<List<Author>> GetAllAuthors()
        {
            return await _dbContext.Authors.ToListAsync();
        }

        public async Task<Author> GetAuthorById(Guid id)
        {
            return await _dbContext.Authors.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<bool> UpdateAuthor(Author author)
        {
            if (author == null) return false;

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
