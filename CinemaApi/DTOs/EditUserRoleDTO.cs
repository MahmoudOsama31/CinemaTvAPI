using System.ComponentModel.DataAnnotations;

namespace CinemaApi.DTOs
{
    public class EditUserRoleDTO
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string RoleId { get; set; }
    }
}
