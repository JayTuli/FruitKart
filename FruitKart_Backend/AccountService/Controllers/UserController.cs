using AccountService.Repository;
using AccountService.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AccountService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository)); 
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userRepository.GetAllUsersAsync();

            if (users == null || !users.Any())
            {
                return NotFound(new ApiResponse()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "No users found." }
                });
            }

            return Ok(new ApiResponse()
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Result = users
            });
        }

        [Authorize]                                                              
        [HttpGet("GetUserById/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound(new ApiResponse()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { $"User with ID {id} not found." }
                });
            }

            return Ok(new ApiResponse()
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Result = user
            });
        }

        [Authorize]                                                
        [HttpPut("UpdateAddress/{id}")]
        public async Task<IActionResult> UpdateUserAddress(int id, [FromBody] UpdateUserDTO updateDTO)
        {
            if (updateDTO == null || string.IsNullOrWhiteSpace(updateDTO.Address))
            {
                return BadRequest(new ApiResponse()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "Invalid address data." }
                });
            }

            var isUpdated = await _userRepository.UpdateUserAsync(id, updateDTO);

            if (!isUpdated)
            {
                return NotFound(new ApiResponse()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { $"User with ID {id} not found." }
                });
            }

            return Ok(new ApiResponse()
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Result = new { message = $"User ID {id}'s address updated successfully." }
            });
        }
    }
}