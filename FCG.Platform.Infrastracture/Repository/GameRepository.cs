using FCG.Platform.Domain.Entities.Dto.GameDto;
using FCG.Platform.Domain.Entities.Entity;
using FCG.Platform.Domain.Interfaces.Repositories;
using FCG.Platform.Infrastracture.Connections;
using Microsoft.EntityFrameworkCore;

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

        public async Task<List<GameResponse>> Get()
        {
            return await _context.Games
            .AsNoTracking()
            .OrderBy(game => game.Id)
            .Select(game => new GameResponse
            {
                Name = game.Name,
                Description = game.Description
            })
            .ToListAsync();
        }

        public async Task<GameEntity?> GetById(int id)
        {
            return await _context.Games.FindAsync(id);
        }
    }
}