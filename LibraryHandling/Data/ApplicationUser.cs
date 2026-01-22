using Microsoft.AspNet.Identity.EntityFramework;

namespace LibraryHandling.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }= string.Empty;
        public string Gender { get; set; }
        public ICollection<Borrowing> Borrowings { get; set; }
    }
}
