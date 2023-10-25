using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaApi.Models
{
    public class MovieLink
    {
        public long Id { get; set; }

        public string Quality { get; set; }

        public int Resolation { get; set; }

        [Required]
        public string MovLink { get; set; }
        [Required]
        public long MovieId { get; set; }
        [ForeignKey("MovieId")]
        public Movie Movie { get; set; }
    }
}
