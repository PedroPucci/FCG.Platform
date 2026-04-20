using FCG.Platform.Domain.Entities.Dto;
using FCG.Platform.Domain.Entities.Entity;
using FCG.Platform.Domain.Interfaces.Repositories;
using FCG.Platform.Infrastracture.Connections;
using Microsoft.EntityFrameworkCore;

namespace FCG.Platform.Infrastracture.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<UserEntity> Add(UserEntity userEntity)
        {
            var result = await _context.UserEntity.AddAsync(userEntity);
            await _context.SaveChangesAsync();

            return result.Entity;
        }

        public UserEntity Update(UserEntity userEntity)
        {
            return _context.UserEntity.Update(userEntity).Entity;
        }

        public async Task<bool> Delete(int id)
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
                })
            .ToListAsync();
        }

        public async Task<UserResponse?> GetById(int id)
        {
            return await _context.UserEntity
            .Where(u => u.Id == id)
            .Select(u => new UserResponse
            {
                Name = u.Name,
                Email = u.Email
            })
            .FirstOrDefaultAsync();
        }

        public async Task<UserEntity?> GetByIdCheck(int id)
        {
            return await _context.UserEntity.FindAsync(id);
        }
    }
}