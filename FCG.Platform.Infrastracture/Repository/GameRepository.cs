using FCG.Platform.Domain.Entities.Entity;
using FCG.Platform.Domain.Interfaces.Repositories;
using FCG.Platform.Infrastracture.Connections;

namespace FCG.Platform.Infrastracture.Repository
{
    public class GameRepository : IGameRepository
    {
        private readonly DataContext _context;

        public GameRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<GameEntity> Add(GameEntity gameEntity)
        {
            var result = await _context.Games.AddAsync(gameEntity);
            await _context.SaveChangesAsync();

            return result.Entity;
        }

        public GameEntity Update(GameEntity gameEntity)
        {
            return _context.Games.Update(gameEntity).Entity;
        }

        public async Task<bool> Delete(int id)
        {
            var game = await GetById(id);

            if (game == null)
                return false;

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<GameEntity>> Get()
        {
            //return await _context.GameEntity
            //.AsNoTracking()
            //.OrderBy(game => game.Id)
            //.Select(game => new GameEntity
            //{
            //    Id = game.Id,                
            //    Name = game.Name,
            //    Description = game.Description
            //})
            //.ToListAsync();
            return null;
        }

        public async Task<GameEntity?> GetById(int id)
        {
            return await _context.Games.FindAsync(id);
        }
    }
}