using FCG.Platform.Domain.Entities.Entity;
using FCG.Platform.Infrastracture.Connections;
using FCG.Platform.Infrastracture.Repository.RepositoryUoW;

namespace FCG.Platform.Infrastracture.Repository
{
    public class UserGameRepository : IUserGameRepository
    {
        private readonly DataContext _context;

        public UserGameRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<UserGameEntity> Add(UserGameEntity userGameEntity)
        {
            var result = await _context.UserGames.AddAsync(userGameEntity);
            await _context.SaveChangesAsync();

            return result.Entity;
        }
    }
}