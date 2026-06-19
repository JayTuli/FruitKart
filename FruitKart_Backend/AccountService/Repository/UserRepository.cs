using AccountService.Models;
using AccountService.Data;
using AccountService.Models.DTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AccountDbContext _userDbContext;
        private readonly IMapper _mapper;

        public UserRepository(AccountDbContext context, IMapper mapper)
        {
            _userDbContext = context ?? throw new ArgumentNullException(nameof(context)); 
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));            
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userDbContext.User                         
                .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userDbContext.User.ToListAsync();      
        }

        public async Task<bool> UpdateUserAsync(int userId, UpdateUserDTO updateDTO)  
        {
            var user = await _userDbContext.User                 
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null) return false;

            _mapper.Map(updateDTO, user);
            await _userDbContext.SaveChangesAsync();
            return true;
        }
    }
}