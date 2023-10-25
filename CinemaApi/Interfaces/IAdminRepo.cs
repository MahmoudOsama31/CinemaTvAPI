using CinemaApi.DTOs;
using CinemaApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace CinemaApi.Interfaces
{
    public interface IAdminRepo
    {
       public Task<IEnumerable<ApplicationUser>>GetAllUsers();
       public Task<ApplicationUser> AddUser(UserDTO user);
       public Task<ApplicationUser> GetUser(string Id);
       public Task<ApplicationUser> EditUser(EditDTO model , string id );
       public Task<ApplicationUser> DeleteUser(string Id);
        // u need to user roles 
        // make a table has username(users) userid(userroles) rolename(roles) roleid(userroles)
        //so we use a join to join three tables
        public Task<IEnumerable<UserRolesModel>> GetUserRolesAsync();
        public Task<bool> EditUserRole(EditUserRoleDTO model);
        public Task<IEnumerable<ApplicationRole>> GetAllRoles();
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Task<IEnumerable<Category>> GetAllCategory();
        public Task<Category> AddCategory(Category model);
        public Task<Category> EditCategory(Category model);
        public Task<Category> DeleteCategory(int Id);
        public Task<Category> GetCategoryById(int Id);
        public Task<IEnumerable<SubCategory>> GetAllSubCategory();
        public Task<SubCategory> AddSubCategory(SubCategory model);
        public Task<SubCategory> EditSubCategory(SubCategory model);
        public Task<SubCategory> DeleteSubCategory(int Id);
        public Task<SubCategory> GetSubCategoryById(int Id);
        public Task<IEnumerable<Actor>> GetAllActors();
        public Task<Actor> AddActor(string actorName , IFormFile img);
        public Task<Actor> GetActor(int id);
        public Task<Actor> EditActor(int id , string actorName , IFormFile img);
        public Task<Actor> DeleteActor(int id);
        public Task<IEnumerable<Movie>> GetAllMovies();
        Task<bool> AddMovieAsync(IFormFile img, IFormFile video, string story, string movieName, string trailer,
           string catId, List<int> ids, string[] links);
        public Task<IEnumerable<Movie>> SearchMovie(string search);
        public Task<Movie> EditMovie(Movie model, IFormFile img);
        public Task<Movie> GetMovie(int id);
        public Task<Movie> DeleteMovie(int id);
        public Task<IEnumerable<MovieLink>> GetAllMovieLinks(string search);
        public Task<MovieLink> AddMovieLink(MovieLink model);
        public Task<MovieLink> GetMovieLinkById(long id);
        public Task<MovieLink> EditMovieLink(MovieLink model, IFormFile video);
        // Delete MovieLink in api and angular ???????????????????????????????????
        public Task<IEnumerable<MovieActor>> GetAllMovieActors(string search);
        public Task<MovieActor> AddMovieActor(MovieActor model);
        public Task<MovieActor> GetMovieActorById(long id);
        public Task<MovieActor> EditMovieActor(MovieActor model);
        // Delete MovieActor in api and angular  ???????????????????????????????????
    }
}
