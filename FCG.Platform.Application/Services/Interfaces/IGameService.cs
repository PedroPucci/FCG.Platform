using FCG.Platform.Domain.Entities.Dto.GameDto;
using FCG.Platform.Domain.Entities.Entity;
using FCG.Platform.Domain.OperationResult;

namespace FCG.Platform.Domain.Interfaces.Services
{
    public interface IGameService
    {
        Task<Result<GameEntity>> Add(GameResponse gameResponse, string userId);
        Task<Result<GameEntity>> Update(int id, UpdateGameRequest updateGameRequest);
        Task<bool> Delete(int id);
        Task<List<GameResponse>> Get();
        Task<Result<GameResponse>> GetById(int id);
    }
}