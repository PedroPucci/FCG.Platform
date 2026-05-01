using FCG.Platform.Domain.Entities.Dto.GameDto;
using FCG.Platform.Domain.Entities.Dto.UserDto;
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
                Log.Information(LogMessages.AddingGameSuccess(gameEntity));
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
            catch (Exception ex) { 
                await transaction.RollbackAsync(); 
                Log.Error(LogMessages.AddingGameError(ex)); 
                return Result<GameEntity>.Error($"Error to add a new Game: {ex.Message}"); 
            }
        }

        public async Task<Result<bool>> Update(int id, UpdateGameRequest updateGameRequest)
        {
            using var transaction = _repositoryUoW.BeginTransaction();

            try
            {
                var game = await _repositoryUoW.GameRepository.GetById(id);

                if (game is null)
                {
                    var message = LogMessages.CannotPerformActionOnGame("update", id);
                    Log.Error(message);
                    return Result<bool>.Error(message);
                }

                game.Name = updateGameRequest.Name;
                game.Description = updateGameRequest.Description;
                game.ModificationDate = DateTime.UtcNow;

                var isValid = await IsValidGameRequest(game);
                if (!isValid.Success)
                    return Result<bool>.Error(isValid.Message);

                _repositoryUoW.GameRepository.Update(game);
                await _repositoryUoW.SaveAsync();
                await transaction.CommitAsync();

                Log.Information(LogMessages.UpdatingSuccessGame(game));
                return Result<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Log.Error(LogMessages.UpdatingErrorGame(ex));
                throw new InvalidOperationException($"Failed to update game with id {id}. See logs for details.", ex);
            }
        }

        public Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<GameResponse>> Get()
        {
            using var transaction = _repositoryUoW.BeginTransaction();

            try
            {
                List<GameResponse> gameEntities = await _repositoryUoW.GameRepository.Get();
                _repositoryUoW.Commit();

                Log.Information(LogMessages.GetAllGameSuccess());
                return gameEntities;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Log.Error(LogMessages.GetAllGameError(ex));
                throw new InvalidOperationException("Error to loading the list Games. See logs for details.", ex);
            }
        }

        public async Task<Result<GameResponse>> GetById(int id)
        {
            using var transaction = _repositoryUoW.BeginTransaction();

            try
            {
                var game = await _repositoryUoW.GameRepository.GetById(id);

                if (game is null)
                {
                    transaction.Rollback();

                    var message = LogMessages.CannotPerformActionOnGame("retrieve", id);
                    Log.Error(message);

                    return Result<GameResponse>.Error(message);
                }

                var gameResponse = new GameResponse
                {
                    Name = game.Name,
                    Description = game.Description
                };

                _repositoryUoW.Commit();

                Log.Information(LogMessages.GetByGameIdSuccess(game));
                return Result<GameResponse>.Ok(gameResponse);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Log.Error(LogMessages.GetByGameIdError(ex));
                throw new InvalidOperationException("Error retrieving the game. See inner exception for details.", ex);
            }
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