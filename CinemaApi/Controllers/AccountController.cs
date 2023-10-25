using CinemaApi.Data;
using CinemaApi.DTOs;
using CinemaApi.Models;
using CinemaApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace CinemaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IEmailService _emailService;
        public AccountController(IEmailService emailService,
               RoleManager<ApplicationRole> roleManager,
               SignInManager<ApplicationUser> signInManager,
               ApplicationContext db,UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailService = emailService;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody]RegisterDTO model)
        {
            if (model==null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                if (EmailExist(model.Email))
                {
                    return BadRequest("Email is not available");
                }
                if (!IsEmailValid(model.Email))
                {
                    return BadRequest("Email is not Valid");
                }
                if (UserNameExist(model.UserName))
                {
                    return BadRequest("UserName is not available");
                }
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email
                };
                var result = await _userManager.CreateAsync(user,model.Password);
                if (result.Succeeded)
                {
                    // i need to confirm his password 
                    return StatusCode(StatusCodes.Status200OK);
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            return BadRequest("Not Valid");

        }
        private bool UserNameExist(string userName)
        {
            if (_db.Users.Any(x => x.UserName == userName))
            {
                return true;
            }
            return false;
        }

        private bool EmailExist(string email)
        {
            if (_db.Users.Any(x=>x.Email==email))
            {
                return true;
            }
            return false;
        }
        //\w+\@+\gmail.com
        private bool IsEmailValid(string email)
        {
            Regex em = new Regex(@"^[a-zA-Z0-9_.+-]+@gmail\.com$");
            if (em.IsMatch(email))
            {
                return true;
            }
            return false;
        }

        [HttpGet]
        [Route("RegistrationConfirm")]
        public async Task<IActionResult> RegistrationConfirm(string ID, string Token)
        {
            if (string.IsNullOrEmpty(ID) || string.IsNullOrEmpty(Token))
                return NotFound();
            var user = await _userManager.FindByIdAsync(ID);
            if (user == null)
                return NotFound();

            var newToken = WebEncoders.Base64UrlDecode(Token);
            var encodeToken = Encoding.UTF8.GetString(newToken);

            var result = await _userManager.ConfirmEmailAsync(user, encodeToken);
            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            await CreateRoles();
            await CreateAdmin();
            if (model==null)
            {
                return NotFound();
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user==null)
            {
                return NotFound();
            }
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);
            if (result.Succeeded)
            {
                if (await _roleManager.RoleExistsAsync("User"))
                {
                    if (!await _userManager.IsInRoleAsync(user,"User")&& !await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        await _userManager.AddToRoleAsync(user, "User");
                    }
                }
                var roleName = await RoleNameByUserId(user.Id);
                if (roleName != null)
                    AddCookies(user.UserName, user.Id, roleName, model.RememberMe,user.Email);
                return StatusCode(StatusCodes.Status200OK);
            }
            else
            {
                return BadRequest("not logined");
            }
        }
        private async Task<string> RoleNameByUserId(string userId)
        {
            var userRole = await _db.UserRoles.FirstOrDefaultAsync(x => x.UserId == userId);

            if (userRole!=null)
            {
                return await _db.Roles.Where(x => x.Id == userRole.RoleId).Select(x => x.Name).FirstOrDefaultAsync();
            }
            return null;
        }
        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetAllUsers()
        {
            return await _db.Users.ToListAsync();
        }
        private async Task CreateAdmin()
        {
            var admin = await _userManager.FindByNameAsync("Admin");
            if (admin==null)
            {
                var user = new ApplicationUser
                {
                    UserName = "Admin",
                    Email = "Admin@gmail.com",
                    EmailConfirmed=true
                };
                var result = await _userManager.CreateAsync(user, "123456");
                if (result.Succeeded)
                {
                    if (await _roleManager.RoleExistsAsync("Admin"))                                       
                        await _userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }

        private async Task CreateRoles()
        {
            if (_roleManager.Roles.Count()<1)
            {
                var role = new ApplicationRole
                {
                    Name = "Admin"
                };
                await _roleManager.CreateAsync(role);

                role = new ApplicationRole
                {
                    Name = "User"
                };
                await _roleManager.CreateAsync(role);
            }
        }
        private async void AddCookies(string userName, string userId, string roleName, bool remember,string email)
        {
            var claim = new List<Claim>
            {
                new Claim(ClaimTypes.Name,userName),
                new Claim(ClaimTypes.Email,email),
                new Claim(ClaimTypes.NameIdentifier,userId),
                new Claim(ClaimTypes.Role,roleName),
            };
            var ClaimIdentity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);
            if (remember)
            {
                var authProperities = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(10)
                };

                await HttpContext.SignInAsync
                    (CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(ClaimIdentity), authProperities);
            }
            else
            {
                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IsPersistent = false,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(ClaimIdentity), authProperties);
            }
        }
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }
       
        [HttpGet("GetRoleName/{email}")]
        public async Task<ActionResult<string>> GetRoleName(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var userRole = await _db.UserRoles.FirstOrDefaultAsync(x => x.UserId == user.Id);
                if (userRole != null)
                {
                    var rolename= await _db.Roles.Where(x => x.Id == userRole.RoleId).Select(x => x.Name).FirstOrDefaultAsync();
                    return Ok(rolename);
                }
            }
            return null;
        }
        [Authorize]
        [HttpGet("CheckUserClaims/{email}&{role}")]
        public IActionResult CheckUserClaims(string email , string role)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userEmail !=null && userRole !=null && id != null)
            {
                if (email == userEmail && role == userRole)
                {
                    return StatusCode(StatusCodes.Status200OK);
                }
            }
            return StatusCode(StatusCodes.Status203NonAuthoritative);
        }
        [HttpGet("ForgetPassword/{email}")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            if (email == null)
            {
                return NotFound();
            }
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodeToken = Encoding.UTF8.GetBytes(token);
            var newToken = WebEncoders.Base64UrlEncode(encodeToken);
            // Generate the password reset link
            ////var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, newToken }, Request.Scheme);
            var confirmLink = $"http://localhost:4200/ResetPassword?ID={user.Id}&Token={newToken}";
            // Send the password reset email
            var emailSubject = "Reset Your Password";
            var emailBody = $"Please reset your password by clicking the link below:\n\n{confirmLink}";
            var address = $"{email}";
            await _emailService.SendEmailAsync(email,address, emailSubject, emailBody);

            return Ok(new{/*userID = user.Id ,*/ Token = newToken });
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            // Validate the password reset token
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return BadRequest("Invalid user.");
            }

            var newToken = WebEncoders.Base64UrlDecode(model.Token);
            var encodeToken = Encoding.UTF8.GetString(newToken);

            var result = await _userManager.ResetPasswordAsync(user, encodeToken, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok("Password has been reset successfully.");
            }

            return BadRequest("Failed to reset password.");
        }
    }
}
