using AccountService.Data;
using AccountService.Models;
using AccountService.Models.DTO;
using AutoMapper;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AccountService.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AccountDbContext _accountDBContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AccountRepository(
            AccountDbContext accountDBContext,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            IMapper mapper)                                 
        {
            _accountDBContext = accountDBContext ?? throw new ArgumentNullException(nameof(accountDBContext));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }


        public async Task<LoginResponseDTO> LoginAsync(LoginDTO login)
        {
            // Find user in Identity (AspNetUsers)
            var validateUser = await _userManager.FindByEmailAsync(login.Email!);
            if (validateUser == null)
            {
                return new LoginResponseDTO() { UserId = 0, Token = null };
            }

            // Validate password
            if (!await _userManager.CheckPasswordAsync(validateUser, login.Password!))
            {
                return new LoginResponseDTO() { UserId = 0, Token = null };
            }

            // Fetch from tbl_User (may be null for Admin)
            var userFromTable = await _accountDBContext.User
                .FirstOrDefaultAsync(u => u.Email == validateUser.Email);

            // Build claims
            var userRoles = await _userManager.GetRolesAsync(validateUser);
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, validateUser.Id),
        new Claim(ClaimTypes.Email, validateUser.Email!),
        new Claim(ClaimTypes.Name, validateUser.Name),
        new Claim("UserId", userFromTable?.UserId.ToString() ?? "0")
    };

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Return response
            return new LoginResponseDTO()
            {
                UserId = userFromTable?.UserId ?? 0,
                Token = GenerateToken(claims)
            };
        }
        //    Register

        public async Task<(int, string)> RegisterAsync(NewUserDTO user, string role)
        {
            // 1: Check if user already exists
            var userExist = await _userManager.FindByEmailAsync(user.Email!);
            if (userExist != null)
            {
                return (0, "User already exists");
            }

            // Step 2: Build ApplicationUser for Identity
            var applicationUser = new ApplicationUser()
            {
                Name = user.Name,                                       
                UserName = user.Email,                                  
                Email = user.Email
            };

            // Step 3: Build tbl_User entry
            var newUser = new User();
            _mapper.Map(user, newUser);

            // Step 4: Wrap both writes in a transaction   
            using var transaction = await _accountDBContext.Database.BeginTransactionAsync();
            try
            {
                // Create Identity user
                var res = await _userManager.CreateAsync(applicationUser, user.Password);
                if (!res.Succeeded)
                {
                    await transaction.RollbackAsync();
                    var errors = string.Join(", ", res.Errors.Select(e => e.Description));
                    return (0, $"User creation failed: {errors}");
                }

                // Create role if it doesn't exist
                var roleExist = await _roleManager.RoleExistsAsync(role);
                if (!roleExist)
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole(role));
                    if (!roleResult.Succeeded)
                    {
                        await transaction.RollbackAsync();
                        var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                        return (0, $"Role creation failed: {errors}");
                    }
                }

                // Assign role to user
                await _userManager.AddToRoleAsync(applicationUser, role);

                // Write to tbl_User                     
                await _accountDBContext.User.AddAsync(newUser);
                await _accountDBContext.SaveChangesAsync();

                await transaction.CommitAsync();
                return (1, "User created successfully");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (0, $"An error occurred: {ex.Message}");
            }
        }


        private string GenerateToken(IEnumerable<Claim> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["JwtSettings:ValidIssuer"],
                Audience = _configuration["JwtSettings:ValidAudience"],
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(claims)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}