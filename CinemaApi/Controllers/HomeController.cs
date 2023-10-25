using CinemaApi.DTOs;
using CinemaApi.Interfaces;
using CinemaApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CinemaApi.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IHomeRepo _repo;

        public HomeController(IHomeRepo repo)
        {
            _repo = repo;
        }
        [HttpGet("GetAllSubCategories")]
        public async Task<IEnumerable<SubCategory>> GetAllSubCategories()
        {
            return await _repo.GetAllSubCategories();
        }
        [HttpGet("GetAllMovies/{search}")]
        public async Task<IEnumerable<Movie>> GetAllMovies(string search)
        {
            return  await _repo.GetAllMovies(search);
        }
        [HttpGet("GetMovie/{id}")]
        public async Task<ActionResult<MovieModel>> GetMovie(long id)
        {
            if(id <0) {
                return BadRequest();
            }
            return await _repo.GetMovieAsync(id);
        }
        [HttpGet("GetAllMoviesByActorId/{id}")]
        public async Task<ActionResult<MovieActor>> GetAllMoviesByActorId(int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }
            var actorMovies =  await _repo.GetAllMoviesByActorId(id);
            return Ok(actorMovies);
        }

    }
}
