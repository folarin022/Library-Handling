using LibraryHandling.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace LibraryHandling.Context
{
    public class LibraryManagementDbContext : DbContext
    {
        public LibraryManagementDbContext(DbContextOptions<LibraryManagementDbContext> options)
           : base(options) 
        {
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Borrowing> Borrowings { get; set; }

    }
}
