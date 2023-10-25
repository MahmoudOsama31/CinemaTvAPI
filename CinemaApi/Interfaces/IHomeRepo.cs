using CinemaApi.DTOs;
using CinemaApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace CinemaApi.Interfaces
{
    public interface IHomeRepo
    {
        public Task<IEnumerable<SubCategory>> GetAllSubCategories();
        public Task<IEnumerable<Movie>> GetAllMovies(string search);
        public Task<ActionResult<MovieModel>> GetMovieAsync(long id);
        public Task<IEnumerable<MovieActor>> GetAllMoviesByActorId(int id);

    }
}
