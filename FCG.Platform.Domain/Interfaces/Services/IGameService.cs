using FCG.Platform.Domain.Entities.Entity;
using FCG.Platform.Domain.OperationResult;

namespace FCG.Platform.Domain.Interfaces.Services
{
    public interface IGameService
    {
        Task<Result<GameEntity>> Add(GameEntity gameEntity);
        Task<Result<GameEntity>> Update(GameEntity gameEntity);
        Task<bool> Delete(int id);
        Task<List<GameEntity>> Get();
        Task<Result<GameEntity>> GetById(int id);
    }
}