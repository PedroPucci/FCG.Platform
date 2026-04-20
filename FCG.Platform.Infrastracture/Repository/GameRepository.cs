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

        public Task<GameEntity> Add(GameEntity gameEntity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<GameEntity>> Get()
        {
            throw new NotImplementedException();
        }

        public Task<GameEntity?> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public GameEntity Update(GameEntity gameEntity)
        {
            throw new NotImplementedException();
        }
    }
}