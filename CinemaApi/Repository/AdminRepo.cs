using CinemaApi.Data;
using CinemaApi.DTOs;
using CinemaApi.Interfaces;
using CinemaApi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaApi.Repository
{
    public class AdminRepo : IAdminRepo
    {
        private readonly ApplicationContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        public AdminRepo(ApplicationContext db , UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager) 
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<Actor> AddActor(string actorName, IFormFile img)
        {
            var FolderPath = Directory.GetCurrentDirectory() +"/wwwroot/images/actors";
            var FileName = Path.GetFileName(img.FileName);
            var FinalPath = Path.Combine(FolderPath, FileName);
            using (var stream = new FileStream(FinalPath,FileMode.Create))
            {
               await img.CopyToAsync(stream);
            }

            var actor = new Actor
            {
                ActorName = actorName,
                ActorPicture = img.FileName
            };
            await _db.Actors.AddAsync(actor);
            await _db.SaveChangesAsync();
            return actor;
        }

        public async Task<Category> AddCategory(Category model)
        {
            if (model == null)
            {
                return null;
            }
            var cat = new Category
            {
                CategoryName = model.CategoryName
            };
            await _db.AddAsync(cat);
            await _db.SaveChangesAsync();
            return cat;
        }

        public async Task<MovieActor> AddMovieActor(MovieActor model)
        {
            if (model == null)
            {
                return null;
            }
            var movieActor = new MovieActor
            {
                ActorId = model.ActorId,
                MovieId = model.MovieId,
            };
            await _db.MovieActors.AddAsync(movieActor);
            await _db.SaveChangesAsync();
            return movieActor;
        }

        public async Task<bool> AddMovieAsync(IFormFile img, IFormFile video, string story, string movieName, string trailer,
            string catId, List<int> ids, string[] links)
        {
            foreach (var mov in await _db.Movies.ToListAsync())
            {
                if (mov.MovieName.Trim().ToLower() == movieName.Trim().ToLower())
                {
                    return false;
                }
            }
            var movie = new Movie
            {
                MovieName = movieName,
                MovieStory = story,
                MovieTrailer = trailer,
                MoviePost = img.FileName,
                SubCategoryId = int.Parse(catId)
            };
            _db.Movies.Add(movie);
            await _db.SaveChangesAsync();
            foreach (var id in ids)
            {
                var movieActors = new MovieActor
                {
                    MovieId = movie.Id,
                    ActorId = id
                };
                _db.MovieActors.Add(movieActors);
                await _db.SaveChangesAsync();
            }
            if (links.Count() >= 0)
            {
                for (int i = 0; i < links.Count(); i++)
                {
                    var movieLink = new MovieLink
                    {
                        MovLink = links[i],
                        MovieId = movie.Id
                    };
                    _db.MovieLinks.Add(movieLink);
                    await _db.SaveChangesAsync();
                }
            }
            var FolderPath = Directory.GetCurrentDirectory() + "/wwwroot/images/actors";
            var FileName = Path.GetFileName(img.FileName);
            var FinalPath = Path.Combine(FolderPath, FileName);
            using (var stream = new FileStream(FinalPath, FileMode.Create))
            {
                await img.CopyToAsync(stream);
            }
             FolderPath = Directory.GetCurrentDirectory() + "/wwwroot/videos/actors";
             FileName = Path.GetFileName(video.FileName);
             FinalPath = Path.Combine(FolderPath, FileName);
            using (var stream = new FileStream(FinalPath, FileMode.Create))
            {
                await video.CopyToAsync(stream);
                var movieLink = new MovieLink
                {
                    MovLink = video.FileName,
                    MovieId = movie.Id
                };
                _db.MovieLinks.Add(movieLink);
                await _db.SaveChangesAsync();
            }
            return true;
        }

        public async Task<MovieLink> AddMovieLink(MovieLink model)
        {
            if (model == null)
            {
                return null;
            }
            var movieLink = new MovieLink
            {
                Resolation = model.Resolation,
                MovieId = model.MovieId,
                MovLink = model.MovLink,
                Quality = model.Quality
            };
            await _db.MovieLinks.AddAsync(movieLink);
            await _db.SaveChangesAsync();
            return movieLink;
        }

        public async Task<SubCategory> AddSubCategory(SubCategory model)
        {
            if(model == null)
            {
                return null;
            }
            var subCategory = new SubCategory { 
                SubCategoryName = model.SubCategoryName,
                CategoryId = model.CategoryId
            };
            await _db.SubCategories.AddAsync(subCategory);
            await _db.SaveChangesAsync();
            return subCategory;
        }

        public async Task<ApplicationUser> AddUser(UserDTO model)
        {
            if (model == null)
            {
                return null;
            }
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = model.EmailConfirmed
            };
            var result = await _userManager.CreateAsync(user,model.Password);
            if (result.Succeeded)
            {
                if (await _roleManager.RoleExistsAsync("User"))
                {
                    if (!await _userManager.IsInRoleAsync(user, "User") && !await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        await _userManager.AddToRoleAsync(user, "User");
                    }
                }
                return user;
            }
            return null;
        }

        public async Task<Actor> DeleteActor(int id)
        {
            if (id < 0)
            {
                return null;
            }
            var actor = await _db.Actors.FirstOrDefaultAsync(x=>x.Id==id);
            if (actor == null)
            {
                return null;
            }
            _db.Remove(actor);
            await _db.SaveChangesAsync();
            return actor;
        }

        public async Task<Category> DeleteCategory(int Id)
        {
            if (Id < 0)
            {
                return null;
            }
            var cat = await _db.Categories.FirstOrDefaultAsync(x=>x.Id==Id);
            if (cat == null)
            {
                return null;
            }
            _db.Remove(cat);
            await _db.SaveChangesAsync();
            return cat;
        }

        public async Task<Movie> DeleteMovie(int id)
        {
            if (id < 0)
            {
                return null;
            }
            var mov = await _db.Movies.FirstOrDefaultAsync(x => x.Id == id);
            if (mov == null)
            {
                return null;
            }
            _db.Remove(mov);
            await _db.SaveChangesAsync();
            return mov;
        }

        public async Task<SubCategory> DeleteSubCategory(int Id)
        {
            if (Id < 0)
            {
                return null;
            }
            var cat = await _db.SubCategories.FirstOrDefaultAsync(x => x.Id == Id);
            if (cat == null)
            {
                return null;
            }
            _db.Remove(cat);
            await _db.SaveChangesAsync();
            return cat;
        }
        public async Task<ApplicationUser> DeleteUser(string Id)
        {
            if (Id == null)
            {
                return null;
            }
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == Id);
            if (user == null)
            {
                return null;
            }
             _db.Remove(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task<Actor> EditActor(int id, string actorName, IFormFile img)
        {
            var actor = await _db.Actors.FirstOrDefaultAsync(x=>x.Id==id);
            if (actor == null)
            {
                return null;
            }
            actor.ActorName = actorName;
            if (img != null && img.FileName.ToLower() != actor.ActorPicture.ToLower())
            {
                var FolderPath = Directory.GetCurrentDirectory() + "/wwwroot/images/actors";
                var FileName = Path.GetFileName(img.FileName);
                var FinalPath = Path.Combine(FolderPath, FileName);
                using (var stream = new FileStream(FinalPath, FileMode.Create))
                {
                    await img.CopyToAsync(stream);
                }
                actor.ActorPicture = img.FileName;
            }          
            _db.Actors.Update(actor);
            await _db.SaveChangesAsync();
            return actor;
        }

        public async Task<Category> EditCategory(Category model)
        {
            if (model == null)
            {
                return null;
            }
            var cat = await _db.Categories.FirstOrDefaultAsync(x=>x.Id==model.Id);
            if (cat == null)
            {
                return null;
            }
            cat.CategoryName = model.CategoryName;
            _db.Categories.Update(cat);
            await _db.SaveChangesAsync();
            return cat;
        }

        public async Task<Movie> EditMovie(Movie model, IFormFile img)
        {
            var mov = await _db.Movies.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (mov == null)
            {
                return null;
            }
            mov.MovieName = model.MovieName;
            mov.MovieStory = model.MovieStory;
            mov.MovieTrailer = model.MovieTrailer;
            mov.SubCategoryId = model.SubCategoryId;
            if (img != null && img.FileName.ToLower() != mov.MoviePost.ToLower())
            {
                var FolderPath = Directory.GetCurrentDirectory() + "/wwwroot/images/actors";
                var FileName = Path.GetFileName(img.FileName);
                var FinalPath = Path.Combine(FolderPath, FileName);
                using (var stream = new FileStream(FinalPath, FileMode.Create))
                {
                    await img.CopyToAsync(stream);
                }
                mov.MoviePost = img.FileName;
            }
            _db.Movies.Update(mov);
            await _db.SaveChangesAsync();
            return mov;
        }
        public async Task<MovieActor> EditMovieActor(MovieActor model)
        {
            if (model == null)
            {
                return null;
            }
            var movActor = await _db.MovieActors.FirstOrDefaultAsync(x => x.Id == model.Id);
            if(movActor == null)
            {
                return null;
            }
            movActor.ActorId = model.ActorId;
            movActor.MovieId = model.MovieId;
            _db.MovieActors.Update(movActor);
            await _db.SaveChangesAsync();
            return movActor;
        }

        public async Task<MovieLink> EditMovieLink(MovieLink model, IFormFile video)
        {
            var movLink = await _db.MovieLinks.FirstOrDefaultAsync(x=>x.Id==model.Id);
            if (movLink == null)
            {
                return null;
            }
            movLink.MovieId = model.MovieId;
            movLink.Resolation = model.Resolation;
            movLink.Quality = model.Quality;
            if (video != null && video.FileName.ToLower() != model.MovLink.ToLower())
            {
                var FolderPath = Directory.GetCurrentDirectory() + "/wwwroot/videos/actors";
                var FileName = Path.GetFileName(video.FileName);
                var FinalPath = Path.Combine(FolderPath, FileName);
                using (var stream = new FileStream(FinalPath, FileMode.Create))
                {
                    await video.CopyToAsync(stream);
                }
                model.MovLink = video.FileName;
            }
            else { model.MovLink = model.MovLink; }
            _db.MovieLinks.Update(movLink);
            await _db.SaveChangesAsync();
            return movLink;
        }

        public async Task<SubCategory> EditSubCategory(SubCategory model)
        {
            if (model == null)
            {
                return null;
            }
            var SubCat = await _db.SubCategories.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (SubCat == null)
            {
                return null;
            }
            SubCat.SubCategoryName = model.SubCategoryName;
            SubCat.CategoryId = model.CategoryId;
            _db.SubCategories.Update(SubCat);
            await _db.SaveChangesAsync();
            return SubCat;
        }
        public async Task<ApplicationUser> EditUser(EditDTO model, string id)
        {
            if (model == null)
            {
                return null;
            }
            var user =await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                return null;
            }
            if (model.Password == user.PasswordHash)
            {
                var result = await _userManager.RemovePasswordAsync(user);
                if (result.Succeeded)
                {
                    await _userManager.AddPasswordAsync(user, model.Password);
                }
            }
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.EmailConfirmed = model.EmailConfirmed;
            user.PhoneNumber = model.PhoneNumber;
            _db.Update(user);
            await _db.SaveChangesAsync();
            return user;

        }
        public async Task<bool> EditUserRole(EditUserRoleDTO model)
        {
            //admin has id , id of admin has roleid , roleid has name , i need to change this name 
            if (model.UserId == null || model.RoleId == null)
            {
                return false;
            }
            var user = await _db.Users.FirstOrDefaultAsync(x=>x.Id==model.UserId);
            if (user == null)
            {
                return false;
            }
            var currentRoleId = await _db.UserRoles.Where(x=>x.UserId==user.Id).Select(x=>x.RoleId).FirstOrDefaultAsync();
            var currentRoleName = await _db.Roles.Where(x=>x.Id==currentRoleId).Select(x=>x.Name).FirstOrDefaultAsync();
            var newRoleName = await _db.Roles.Where(x=>x.Id==model.RoleId).Select(x=>x.Name).FirstOrDefaultAsync();

            if (await _userManager.IsInRoleAsync(user,currentRoleName))
            {
              var x= await _userManager.RemoveFromRoleAsync(user, currentRoleName);
                if (x.Succeeded)
                {
                  var s = await _userManager.AddToRoleAsync(user, newRoleName);
                    if (s.Succeeded)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public async Task<Actor> GetActor(int id)
        {
            if (id < 1)
            {
                return null;
            }
            var actor = await _db.Actors.FirstOrDefaultAsync(x => x.Id == id);
            if (actor == null)
            {
                return null;
            }
            return actor;
        }

        public async Task<IEnumerable<Actor>> GetAllActors()
        {
            return await _db.Actors.ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetAllCategory()
        {
            return await _db.Categories.ToListAsync();
        }

        public async Task<IEnumerable<MovieActor>> GetAllMovieActors(string search)
        {
            if (search == null || search == "null" || string.IsNullOrEmpty(search))
                return await _db.MovieActors.Include(x => x.Actor).Include(x => x.Movie).ThenInclude(x => x.SubCategory).ToListAsync();
            else
            {
                //filmname and actor of this film
                return await _db.MovieActors.Include(x => x.Actor).Include(x => x.Movie).ThenInclude(x => x.SubCategory)
                     .Where(x => x.Movie.MovieName.ToLower().Contains(search.ToLower()) || x.Actor.ActorName.ToLower()
                     .Contains(search.ToLower())).ToListAsync();
            }
        }

        public async Task<IEnumerable<MovieLink>> GetAllMovieLinks(string search)
        {
            if (search == null || search == "null" || string.IsNullOrEmpty(search))
                return await _db.MovieLinks.Include(x => x.Movie).ThenInclude(x => x.SubCategory).ToListAsync();
            else
            { 
            return await _db.MovieLinks.OrderByDescending(x => x.Id).Include(x => x.Movie).ThenInclude(x=>x.SubCategory)
                .Where(x=>x.Movie.MovieName.ToLower().Contains(search.ToLower())|| x.MovLink.ToLower().Contains(search.ToLower())).ToListAsync();
            }
        }

        public async Task<IEnumerable<Movie>> GetAllMovies()
        {
            return await _db.Movies.OrderByDescending(x=>x.Id).Include(x=>x.SubCategory).ToListAsync();
        }

        public async Task<IEnumerable<ApplicationRole>> GetAllRoles()
        {
            return await _db.Roles.ToListAsync();
        }

        public async Task<IEnumerable<SubCategory>> GetAllSubCategory()
        {
            return await _db.SubCategories.Include(x=>x.Category).ToListAsync();
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsers()
        {
            return await _db.Users.ToListAsync();
        }

        public async Task<Category> GetCategoryById(int Id)
        {
            if (Id < 0)
            {
                return null;
            }
            var cat = await _db.Categories.SingleOrDefaultAsync(x=>x.Id==Id);
            if (cat == null)
            {
                return null;
            }
            return cat;
        }

        public async Task<Movie> GetMovie(int id)
        {
            if (id < 1)
            {
                return null;
            }
            var mov = await _db.Movies.Include(x=>x.SubCategory).FirstOrDefaultAsync(x => x.Id == id);
            if (mov == null)
            {
                return null;
            }
            return mov;
        }

        public async Task<MovieActor> GetMovieActorById(long id)
        {
            if (id < 1)
            {
                return null;
            }
            var movActor = await _db.MovieActors.FirstOrDefaultAsync(x=>x.Id==id);
            if (movActor == null)
            {
                return null;
            }
            return movActor;
        }

        public async Task<MovieLink> GetMovieLinkById(long id)
        {
            if (id < 1)
            {
                return null;
            }
            var movLink = await _db.MovieLinks.Include(x => x.Movie).FirstOrDefaultAsync(x=>x.Id==id);
            if (movLink == null)
            {
                return null;
            }
            return movLink;
        }

        public async Task<SubCategory> GetSubCategoryById(int Id)
        {
            if (Id < 0)
            {
                return null;
            }
            var cat = await _db.SubCategories.SingleOrDefaultAsync(x => x.Id == Id);
            if (cat == null)
            {
                return null;
            }
            return cat;
        }

        public async Task<ApplicationUser> GetUser(string Id)
        {
            if (Id == null)
            {
                return null;
            }
            var user = await _db.Users.SingleOrDefaultAsync(x=>x.Id==Id);
            if (user == null)
            {
                return null;
            }
            return user;
        }
        public async Task<IEnumerable<UserRolesModel>> GetUserRolesAsync()
        {
            // user linq
            // make a table has username(users) userid(userroles) rolename(roles) roleid(userroles)
            var query = await (
                from userRole in _db.UserRoles 
                join users in _db.Users
                on userRole.UserId equals users.Id
                join roles in _db.Roles
                on userRole.RoleId equals roles.Id
                select new
                {
                    userRole.UserId,
                    users.UserName,
                    userRole.RoleId,
                    roles.Name
                }).ToListAsync();
            List<UserRolesModel> userRolesModels = new List<UserRolesModel>();
            if (query.Count > 0)
            {
                for (int i = 0; i < query.Count; i++)
                {
                    var model = new UserRolesModel
                    {
                        RoleId = query[i].RoleId,
                        UserId = query[i].UserId,
                        RoleName = query[i].Name,
                        UserName = query[i].UserName
                    };
                    userRolesModels.Add(model);
                }
            }
            return userRolesModels;
        }

        public async Task<IEnumerable<Movie>> SearchMovie(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return null;
            }
            var s = await _db.Movies.OrderByDescending(x => x.Id).Include(x => x.SubCategory)
                .Where(x => x.MovieName.ToLower().Contains(search.ToLower()) || x.SubCategory.SubCategoryName.ToLower()
                .Contains(search.ToLower())).ToListAsync();
            return s;
        }
    }
}
