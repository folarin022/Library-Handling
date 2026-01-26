using LibraryHandling.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;  
using Microsoft.EntityFrameworkCore;

namespace LibraryHandling.Context
{
    public class LibraryManagementDbContext : IdentityDbContext<ApplicationUser>
    {
        public LibraryManagementDbContext(DbContextOptions<LibraryManagementDbContext> options)
            : base(options)
        {
        }

        // Your custom DbSets
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Borrowing> Borrowings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Important for Identity
            // Your custom configurations here
        }
    }
}