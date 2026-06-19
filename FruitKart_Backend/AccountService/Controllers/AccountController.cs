using AccountService.Models;
using AccountService.Models.DTO;
using AccountService.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AccountService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;

        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        }

        [HttpPost("register")]                                      
        public async Task<IActionResult> Register([FromBody] NewUserDTO user)
        {
            try
            {
                var (status, message) = await _accountRepository.RegisterAsync(user, UserRoles.User);
                //var (status, message) = await _accountRepository.RegisterAsync(user, "Customer");

                if (status == 1)
                {
                    return Ok(new ApiResponse()
                    {
                        StatusCode = HttpStatusCode.OK,
                        IsSuccess = true,
                        Result = new { message }
                    });
                }

                return BadRequest(new ApiResponse()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { message }
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponse()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "An unexpected error occurred." }
                });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            var response = await _accountRepository.LoginAsync(login);

            if (response.Token == null)
            {
                return Unauthorized(new ApiResponse()
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "Invalid credentials" }
                });
            }

            return Ok(new ApiResponse()
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Result = new { response.UserId, response.Token }
            });
        }

        //[HttpPost("seed-admin")]
        //[AllowAnonymous]
        //public async Task<IActionResult> SeedAdmin()
        //{
        //    try
        //    {
        //        // Check if Admin role exists, create if not
        //        var roleManager = HttpContext.RequestServices.GetRequiredService<RoleManager<IdentityRole>>();
        //        var userManager = HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();

        //        if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
        //        {
        //            await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
        //        }

        //        if (!await roleManager.RoleExistsAsync(UserRoles.User))
        //        {
        //            await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
        //        }

        //        // Check if admin already exists
        //        var existing = await userManager.FindByEmailAsync("admin@fruitkart.com");
        //        if (existing != null)
        //        {
        //            return BadRequest(new ApiResponse()
        //            {
        //                StatusCode = HttpStatusCode.BadRequest,
        //                IsSuccess = false,
        //                ErrorMessages = new List<string> { "Admin already exists." }
        //            });
        //        }

        //        // Create admin user
        //        var adminUser = new ApplicationUser
        //        {
        //            Name = "Admin",
        //            UserName = "admin@fruitkart.com",
        //            Email = "admin@fruitkart.com",
        //            EmailConfirmed = true
        //        };

        //        var result = await userManager.CreateAsync(adminUser, "Admin@123");

        //        if (!result.Succeeded)
        //        {
        //            return BadRequest(new ApiResponse()
        //            {
        //                StatusCode = HttpStatusCode.BadRequest,
        //                IsSuccess = false,
        //                ErrorMessages = result.Errors.Select(e => e.Description).ToList()
        //            });
        //        }

        //        await userManager.AddToRoleAsync(adminUser, UserRoles.Admin);

        //        return Ok(new ApiResponse()
        //        {
        //            StatusCode = HttpStatusCode.OK,
        //            IsSuccess = true,
        //            Result = new { message = "Admin seeded successfully. Delete this endpoint now!" }
        //        });
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(500, new ApiResponse()
        //        {
        //            StatusCode = HttpStatusCode.InternalServerError,
        //            IsSuccess = false,
        //            ErrorMessages = new List<string> { "An unexpected error occurred." }
        //        });
        //    }
        //}
    }
}