using System.ComponentModel.DataAnnotations;

namespace LibraryHandling.Dto.AuthorModel
{
    public class LoginDto
    {
        [Required]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
