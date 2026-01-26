//using Microsoft.AspNet.Identity.EntityFramework;

using Microsoft.AspNetCore.Identity;

namespace LibraryHandling.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; internal set; }
        public string LastName { get; internal set; }= string.Empty;
        public string? Gender { get; internal set; }
        public ICollection<Borrowing> Borrowings { get; set; }
    }
}
