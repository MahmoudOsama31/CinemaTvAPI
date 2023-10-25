using CinemaApi.Data;
using CinemaApi.DTOs;
using CinemaApi.Interfaces;
using CinemaApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaApi.Repository
{
    public class HomeRepo : IHomeRepo
    {
        private readonly ApplicationContext _db;

        public HomeRepo(ApplicationContext db)
        {
            _db = db;
        }
        public async Task<IEnumerable<Movie>> GetAllMovies(string search)
        {
            if (search == null || string.IsNullOrEmpty(search) || string.IsNullOrWhiteSpace(search) || search == "null")
            {
                return await _db.Movies.OrderByDescending(x => x.Id).Include(x => x.SubCategory).ThenInclude(x => x.Category).ToListAsync();
            }
            else
            {
                //i want to search moviename and subcategory name
                return await _db.Movies.OrderByDescending(x=>x.Id).Include(x=>x.SubCategory).ThenInclude(x=>x.Category)
                    .Where(x=>x.MovieName.ToLower().Contains(search.ToLower())||x.SubCategory.SubCategoryName.ToLower()
                    .Contains(search.ToLower())).ToListAsync();
            }
        }
        public async Task<IEnumerable<SubCategory>> GetAllSubCategories()
        {
            return await _db.SubCategories.ToListAsync();
        }

        public async Task<ActionResult<MovieModel>> GetMovieAsync(long id)
        {
            var mov = await _db.Movies.Include(x => x.SubCategory).FirstOrDefaultAsync(x => x.Id == id);
            if (mov == null)
            {
                return null;
            }
            var actors = await _db.MovieActors.Include(x => x.Actor).Where(x => x.MovieId == mov.Id).ToListAsync();
            var links = await _db.MovieLinks.Where(x => x.MovieId == mov.Id).ToListAsync();
            var model = new MovieModel
            {
                Movie = mov ,
                Actors = actors ,
                Links = links
            };
            return model;
        }
        public async Task<IEnumerable<MovieActor>> GetAllMoviesByActorId(int id)
        {
            return await _db.MovieActors.OrderByDescending(x => x.Id)
                .Include(x => x.Movie).Include(x => x.Actor)
                .Where(x => x.ActorId == id).ToListAsync();
        }
    }
}
