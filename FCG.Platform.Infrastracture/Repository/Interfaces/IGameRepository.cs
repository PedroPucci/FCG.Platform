using FCG.Platform.Domain.Entities.Dto.GameDto;
using FCG.Platform.Domain.Entities.Entity;

namespace FCG.Platform.Domain.Interfaces.Repositories
{
    public interface IGameRepository
    {
        Task<GameEntity> Add(GameEntity gameEntity);
        GameEntity Update(GameEntity gameEntity);
        Task<bool> Delete(int id);
        Task<List<GameResponse>> Get();
        Task<GameEntity?> GetById(int id);
    }
}