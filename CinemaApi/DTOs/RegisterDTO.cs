using System.ComponentModel.DataAnnotations;

namespace CinemaApi.DTOs
{
    public class RegisterDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [StringLength(maximumLength:100,MinimumLength =6)]
        public string Password { get; set; }
    }
}
