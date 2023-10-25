using CinemaApi.Models;

namespace CinemaApi.DTOs
{
    public class MovieModel
    {
        public Movie Movie { get; set; }
        public IEnumerable<MovieActor> Actors { get; set; }
        public IEnumerable<MovieLink> Links { get; set; }
    }
}
