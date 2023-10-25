using CinemaApi.DTOs;
using CinemaApi.Interfaces;
using CinemaApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;
using System;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace CinemaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepo _repo;

        public AdminController(IAdminRepo repo)
        {
            _repo = repo;
        }
        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetAllUsers()
        {
            var users = await _repo.GetAllUsers();
            if (users == null)
            {
                return NotFound();
            }
            return Ok(users);
        }
        [HttpPost("AddUsers")]
        public async Task<IActionResult> AddUserAsync(UserDTO model)
        {
            if (ModelState.IsValid)
            {
                var user = await _repo.AddUser(model);
                if (user != null)
                {
                    return Ok();
                }
            }
            return BadRequest();
        }
        [HttpGet("GetUser/{id}")]
        public async Task<ActionResult<ApplicationUser>> GetUser(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _repo.GetUser(id);
            if (user != null)
            {
                return user;
            }
            return BadRequest();
        }
        [HttpPut("EditUser/{id}")]
        public async Task<ActionResult<ApplicationUser>> EditUser([FromBody]EditDTO model,[FromRoute]string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
           var user =  await _repo.EditUser(model,id);
            if (user != null)
            {
                return user;
            }
            return BadRequest();
        }
        [HttpDelete("DeleteUser/{id}")]
        public async Task<ActionResult<ApplicationUser>> DeleteUser([FromRoute]string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            await _repo.DeleteUser(id);
            return Ok();
        }
        [HttpGet("GetUserRole")]
        public async Task<IEnumerable<UserRolesModel>> GetUserRole()
        {
            var userRoles = await _repo.GetUserRolesAsync();
            if (userRoles == null)
            {
                return null;
            }
            return userRoles;
        }
        [HttpPut("EditUserRole")]
        public async Task<ActionResult<bool>> EditUserRole([FromBody] EditUserRoleDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var userRoleName = await _repo.EditUserRole(model);
            if (userRoleName)
            {
                return userRoleName;
            }
            return BadRequest();
        }
        [HttpGet("GetAllRoles")]
        public async Task<ActionResult<IEnumerable<ApplicationRole>>> GetAllRoles()
        {
            var roles = await _repo.GetAllRoles();
            if (roles == null)
            {
                return NotFound();
            }
            return Ok(roles);
        }
        [HttpGet("GetCategories")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            var category = await _repo.GetAllCategory();
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }
        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory(Category model)
        {
            if (ModelState.IsValid)
            {
                var cat = await _repo.AddCategory(model);
                if (cat != null)
                {
                    return Ok();
                }
            }
            return BadRequest();
        }
        [HttpPut("EditCategory")]
        public async Task<IActionResult> EditCategory(Category model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            var cat = await _repo.EditCategory(model);
            if (cat != null)
            {
                return Ok();
            }
            return BadRequest();
        }
        [HttpDelete("DeleteCategory/{id}")]
        public async Task<ActionResult<ApplicationUser>> DeleteCategory([FromRoute] int id)
        {
            if (id < 0)
            {
                return NotFound();
            }
            await _repo.DeleteCategory(id);
            return Ok();
        }
        [HttpGet("GetCategoryById/{id}")]
        public async Task<ActionResult<Category>> GetCategoryById(int id)
        {
            if (id < 0)
            {
                return NotFound();
            }
            var category = await _repo.GetCategoryById(id);
            if (category != null)
            {
                return category;
            }
            return BadRequest();
        }
        [HttpGet("getAllSubCategories")]
        public async Task<ActionResult<IEnumerable<Category>>> getAllSubCategories()
        {
            var subCategory = await _repo.GetAllSubCategory();
            if (subCategory == null)
            {
                return NotFound();
            }
            return Ok(subCategory);
        }
        [HttpPost("AddSubCategory")]
        public async Task<IActionResult> AddSubCategory(SubCategory model)
        {
            if (ModelState.IsValid)
            {
                var cat = await _repo.AddSubCategory(model);
                if (cat != null)
                {
                    return Ok();
                }
            }
            return BadRequest();
        }
        [HttpPut("EditSubCategory")]
        public async Task<IActionResult> EditSubCategory(SubCategory model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            var cat = await _repo.EditSubCategory(model);
            if (cat != null)
            {
                return Ok();
            }
            return BadRequest();
        }
        [HttpDelete("DeleteSubCategory/{id}")]
        public async Task<ActionResult<SubCategory>> DeleteSubCategory([FromRoute] int id)
        {
            if (id < 0)
            {
                return NotFound();
            }
            await _repo.DeleteSubCategory(id);
            return Ok();
        }
        [HttpGet("GetSubCategoryById/{id}")]
        public async Task<ActionResult<SubCategory>> GetSubCategoryById(int id)
        {
            if (id < 0)
            {
                return NotFound();
            }
            var category = await _repo.GetSubCategoryById(id);
            if (category != null)
            {
                return category;
            }
            return BadRequest();
        }
        [HttpGet("GetAllActors")]
        public async Task<ActionResult<IEnumerable<Actor>>> GetAllActors()
        {
            var Actor = await _repo.GetAllActors();
            if (Actor == null)
            {
                return NotFound();
            }
            return Ok(Actor);
        }
        [HttpPost("AddActor")]
        public async Task<IActionResult> AddActor()
        {
            var actorName = HttpContext.Request.Form["actorName"];
            var img = HttpContext.Request.Form.Files["Image"];
            if (!string.IsNullOrEmpty(actorName) && img != null && img.Length > 0)
            {
                var actor = await _repo.AddActor(actorName, img);
                if (actor != null)
                {
                    return Ok();
                }              
            }
            return BadRequest();
        }
        [HttpGet("GetActor/{id}")]
        public async Task<ActionResult<Actor>> GetActor([FromRoute]int id)
        {
            if(id < 1)
            {
                return BadRequest();
            }
            var actor = await _repo.GetActor(id);
            if (actor != null)
            {
                return actor;
            }
            return BadRequest();
        }
        [HttpPut("EditActor")]
        public async Task<ActionResult<Actor>> EditActor()
        {
            var actorName = HttpContext.Request.Form["actorName"].ToString();
            int id = int.Parse(HttpContext.Request.Form["id"].ToString());
            var img = HttpContext.Request.Form.Files["image"];
            if (!string.IsNullOrEmpty(actorName) && img != null && img.Length > 0)
            {
                var actor = await _repo.EditActor(id, actorName, img);
                if (actor != null)
                {
                    return actor;
                }
                return BadRequest();
            }
            return BadRequest();
        }
        [HttpDelete("DeleteActor/{id}")]
        public async Task<ActionResult<Actor>> DeleteActor([FromRoute] int id)
        {
            if (id < 0)
            {
                return NotFound();
            }
            await _repo.DeleteActor(id);
            return Ok();
        }
        [HttpGet("GetAllMovies")]
        public async Task<ActionResult<IEnumerable<Actor>>> GetAllMovies()
        {
            var movies = await _repo.GetAllMovies();
            if (movies == null)
            {
                return NotFound();
            }
            return Ok(movies);
        }
        [HttpPost("AddMovie")]
        public async Task<IActionResult> AddMovie()
        {
            var img = HttpContext.Request.Form.Files["image"];
            var video = HttpContext.Request.Form.Files["video"];
            var story = HttpContext.Request.Form["story"].ToString();
            var movieName = HttpContext.Request.Form["movieName"].ToString();
            var trailer = HttpContext.Request.Form["trailer"].ToString();
            var catId = HttpContext.Request.Form["catId"].ToString();
            var actorsId = HttpContext.Request.Form["actorsId[]"].ToArray();
            var links = HttpContext.Request.Form["links[]"].ToArray();
            List<int> ids = new List<int>();
            for (int i = 0; i < actorsId.Length; i++)
            {
                var result = int.TryParse(actorsId[i], out int id);
                if (result)
                    ids.Add(id);
            }

            if (ids.Count < 1)
            {
                return NoContent();
            }
            if (img != null && video != null && !string.IsNullOrEmpty(story) && !string.IsNullOrEmpty(movieName) && !
               string.IsNullOrEmpty(trailer) && !string.IsNullOrEmpty(catId) && ids.Count > 0)
            {
                var result = await _repo.AddMovieAsync(img, video, story, movieName, trailer, catId, ids, links);
                if (result)
                {
                    return Ok();
                }
            }
            return BadRequest();
        }
        [HttpGet("SearchMovie/{search}")]
        public async Task<IEnumerable<Movie>> GetMovie(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return null;
            }
            var s = await _repo.SearchMovie(search);
            return s;
        }
        [HttpPut("EditMovie")]
        public async Task<ActionResult<Movie>> EditMovie()
        {
            var img = HttpContext.Request.Form.Files["image"];
            var bodyId = HttpContext.Request.Form["id"].ToString();
            var story = HttpContext.Request.Form["story"].ToString();
            var movieName = HttpContext.Request.Form["movieName"].ToString();
            var trailer = HttpContext.Request.Form["trailer"].ToString();
            var subCatId = HttpContext.Request.Form["subCatId"].ToString();


            var isId = long.TryParse(bodyId, out long id);
            var isSubCatId = int.TryParse(subCatId, out int subId);
            if (!isId || !isSubCatId)
                return BadRequest();
            if (img != null && !string.IsNullOrEmpty(story) && !string.IsNullOrEmpty(movieName)
                && !string.IsNullOrEmpty(trailer))
            {
                var movie = new Movie
                {
                    Id = id,
                    MovieName = movieName,
                    MovieStory = story,
                    MoviePost = img.FileName,
                    SubCategoryId = subId,
                    MovieTrailer = trailer
                };
                var mov = await _repo.EditMovie(movie,img);
                if (mov != null)
                {
                    return mov;
                }
                return BadRequest();
            }
            return BadRequest();
        }
        [HttpGet("GetMovie/{id}")]
        public async Task<ActionResult<Movie>> GetMovie([FromRoute] int id)
        {
            if (id < 1)
            {
                return BadRequest();
            }
            var mov = await _repo.GetMovie(id);
            if (mov != null)
            {
                return mov;
            }
            return BadRequest();
        }
        [HttpDelete("DeleteMovie/{id}")]
        public async Task<ActionResult<Actor>> DeleteMovie([FromRoute] int id)
        {
            if (id < 0)
            {
                return NotFound();
            }
            await _repo.DeleteMovie(id);
            return Ok();
        }
        [HttpGet("GetAllMovieLinks/{search}")]
        public async Task<ActionResult<IEnumerable<MovieLink>>> GetAllMovieLinks(string search)
        {
            var movieLinks= await _repo.GetAllMovieLinks(search);
            if (movieLinks == null)
            {
                return NotFound();
            }
            return Ok(movieLinks);
        }
        [HttpPost("AddMovieLink")]
        public async Task<IActionResult> AddMovieLink(MovieLink model)
        {
            if (ModelState.IsValid)
            {
                var movLink = await _repo.AddMovieLink(model);
                if (movLink != null)
                {
                    return Ok();
                }
            }
            return BadRequest();
        }
        [HttpPut("EditMovieLink")]
        public async Task<ActionResult<MovieLink>> EditMovieLink()
        {
            var video = HttpContext.Request.Form.Files["video"];
            var ilinkId = HttpContext.Request.Form["id"].ToString();
            var quality = HttpContext.Request.Form["quality"].ToString();
            var res = HttpContext.Request.Form["resolation"].ToString();
            var link = HttpContext.Request.Form["movLink"].ToString();
            var movId = HttpContext.Request.Form["movieId"].ToString();
            var isId = long.TryParse(ilinkId, out long id);
            var isMovId = long.TryParse(movId, out long movieId);
            int.TryParse(res, out int resolation);

            if (!isId || !isMovId)
                return BadRequest();
            var movLink = new MovieLink
            {
                Id = id,
                MovLink = link,
                Quality = quality,
                Resolation = resolation,
                MovieId = movieId
            };
            var mov = await _repo.EditMovieLink(movLink, video);
            if (mov != null)
            {
                return movLink;
            }
            return BadRequest();
        }
        [HttpGet("GetMovieLinkById/{id}")]
        public async Task<ActionResult<MovieLink>> GetMovieLinkById(int id)
        {
            if (id < 0)
            {
                return NotFound();
            }
            var movLink = await _repo.GetMovieLinkById(id);
            if (movLink != null)
            {
                return movLink;
            }
            return BadRequest();
        }
        [HttpGet("GetAllMovieActors/{search}")]
        public async Task<ActionResult<IEnumerable<MovieActor>>> GetAllMovieActors(string search)
        {
            var movieActors = await _repo.GetAllMovieActors(search);
            if (movieActors == null)
            {
                return NotFound();
            }
            return Ok(movieActors);
        }
        [HttpPost("AddMovieActor")]
        public async Task<IActionResult> AddMovieActor(MovieActor model)
        {
            if (ModelState.IsValid)
            {
                var movActor = await _repo.AddMovieActor(model);
                if (movActor != null)
                {
                    return Ok();
                }
            }
            return BadRequest();
        }
        [HttpGet("GetMovieActorById/{id}")]
        public async Task<ActionResult<MovieActor>> GetMovieActorById(int id)
        {
            if (id < 0)
            {
                return NotFound();
            }
            var MovieActor = await _repo.GetMovieActorById(id);
            if (MovieActor != null)
            {
                return MovieActor;
            }
            return BadRequest();
        }
        [HttpPut("EditMovieActor")]
        public async Task<ActionResult<MovieActor>> EditMovieActor(MovieActor model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _repo.EditMovieActor(model);
            if (result!=null)
            {
                return Ok();
            }
            return BadRequest();
        }   
    }
}
