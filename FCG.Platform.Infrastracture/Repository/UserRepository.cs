using FCG.Platform.Domain.Entities.Dto.UserDto;
using FCG.Platform.Domain.Entities.Entity;
using FCG.Platform.Domain.Interfaces.Repositories;
using FCG.Platform.Infrastracture.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FCG.Platform.Infrastracture.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly UserManager<UserEntity> _userManager;

        public UserRepository(DataContext context, UserManager<UserEntity> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<UserEntity> Add(UserEntity userEntity)
        {
            var result = await _userManager.CreateAsync(userEntity, userEntity.PasswordHash);

            if (!result.Succeeded)
                throw new InvalidOperationException(
                    string.Join(" | ", result.Errors.Select(e => e.Description)));

            return userEntity;
        }

        public UserEntity Update(UserEntity userEntity)
        {
            return _context.UserEntity.Update(userEntity).Entity;
        }

        public async Task<bool> Delete(string id)
        {
            var user = await GetByIdCheck(id);

            if (user == null)
                return false;

            _context.UserEntity.Remove(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<UserResponse>> Get()
        {
            return await _context.UserEntity
                .AsNoTracking()
                .OrderBy(user => user.Id)
                .Select(user => new UserResponse
                {
                    Email = user.Email,
                    Name = user.Name,
                    IsActive = user.IsActive,                    
                })
            .ToListAsync();
        }

        public async Task<UserEntity> GetByEmail(string email)
        {
            var result = await _userManager.FindByEmailAsync(email);
            return result;
        }

        public async Task<bool> CheckPassword(UserEntity userEntity, string password)
        {
            var result = await _userManager.CheckPasswordAsync(userEntity, password);
            return result;
        }

        public async Task<UserEntity?> GetByIdCheck(string id)
        {
            return await _context.UserEntity.FindAsync(id);
        }
    }
}