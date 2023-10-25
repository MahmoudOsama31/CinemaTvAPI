using System.ComponentModel.DataAnnotations;

namespace CinemaApi.Models
{
    public class UserRolesModel
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string RoleId { get; set; }
        [Required]
        public string RoleName { get; set; }

    }
}
