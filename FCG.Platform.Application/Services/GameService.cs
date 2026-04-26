using FCG.Platform.Domain.Entities.Dto.GameDto;
using FCG.Platform.Domain.Entities.Entity;
using FCG.Platform.Domain.Interfaces.Services;
using FCG.Platform.Domain.OperationResult;
using FCG.Platform.Infrastracture.Repository.RepositoryUoW;
using FCG.Platform.Shared.Logging;
using FCG.Platform.Shared.Validator;
using Serilog;

namespace FCG.Platform.Application.Services
{
    public class GameService : IGameService
    {
        private readonly IRepositoryUoW _repositoryUoW;

        public GameService(IRepositoryUoW repositoryUoW)
        {
            _repositoryUoW = repositoryUoW;
        }

        public async Task<Result<GameEntity>> Add(GameResponse gameResponse, string userId)
        {
            using var transaction = _repositoryUoW.BeginTransaction();

            try
            {
                var gameEntity = new GameEntity
                {
                    Name = gameResponse.Name,
                    Description = gameResponse.Description,
                    CreateDate = DateTime.UtcNow,
                    IsActive = true
                };

                var isValid = await IsValidGameRequest(gameEntity);
                if (!isValid.Success)
                    return Result<GameEntity>.Error(isValid.Message);

                await _repositoryUoW.GameRepository.Add(gameEntity);
                await _repositoryUoW.SaveAsync();

                var userGameEntity = new UserGameEntity
                {
                    UserId = userId,
                    GameId = gameEntity.Id
                };

                await _repositoryUoW.UserGameRepository.Add(userGameEntity);
                await _repositoryUoW.SaveAsync();

                await transaction.CommitAsync();

                return Result<GameEntity>.Ok(gameEntity);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Result<GameEntity>.Error($"Error to add a new Game: {ex.InnerException?.Message ?? ex.Message}");
            }
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

        private async Task<Result<GameEntity>> IsValidGameRequest(GameEntity gameEntity)
        {
            var requestValidator = await new GameRequestValidator().ValidateAsync(gameEntity);

            if (!requestValidator.IsValid)
            {
                string errorMessage = string.Join(" ", requestValidator.Errors.Select(e => e.ErrorMessage));
                errorMessage = errorMessage.Replace(Environment.NewLine, "");
                return Result<GameEntity>.Error(errorMessage);
            }

            return Result<GameEntity>.Ok();
        }
    }
}