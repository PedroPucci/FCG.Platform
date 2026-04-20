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
            var user = await GetById(id);

            if (user == null)
                return false;

            _context.UserEntity.Remove(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<UserEntity>> Get()
        {
            return await _context.UserEntity
                .AsNoTracking()
                .OrderBy(user => user.Id)
                .Select(user => new UserEntity
                {
                    Id = user.Id,
                    Email = user.Email,
                    IsActive = user.IsActive
                })
            .ToListAsync();
        }

        public async Task<UserEntity?> GetById(int id)
        {
            return await _context.UserEntity.FindAsync(id);
        }
    }
}