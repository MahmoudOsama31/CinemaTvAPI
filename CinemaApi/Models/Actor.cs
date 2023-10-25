using System.ComponentModel.DataAnnotations;

namespace CinemaApi.Models
{
    public class Actor
    {
        public int Id { get; set; }

        [Required]
        public string ActorName { get; set; }

        public string ActorPicture { get; set; }
    }
}
