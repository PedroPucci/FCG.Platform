using FCG.Platform.Domain.Entities.Entity;
using FCG.Platform.Domain.Interfaces.Services;
using FCG.Platform.Domain.OperationResult;
using FCG.Platform.Infrastracture.Repository.RepositoryUoW;

namespace FCG.Platform.Application.Services
{
    public class GameService : IGameService
    {
        private readonly IRepositoryUoW _repositoryUoW;

        public GameService(IRepositoryUoW repositoryUoW)
        {
            _repositoryUoW = repositoryUoW;
        }

        public Task<Result<GameEntity>> Add(GameEntity gameEntity)
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

        public Task<Result<GameEntity>> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Result<GameEntity>> Update(GameEntity gameEntity)
        {
            throw new NotImplementedException();
        }
    }
}