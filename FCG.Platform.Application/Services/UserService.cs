using FCG.Platform.Domain.Entities.Entity;
using FCG.Platform.Domain.Interfaces.Services;
using FCG.Platform.Domain.OperationResult;
using FCG.Platform.Infrastracture.Repository.RepositoryUoW;
using FCG.Platform.Shared.Logging;
using FCG.Platform.Shared.Validator;
using Serilog;

namespace FCG.Platform.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IRepositoryUoW _repositoryUoW;

        public UserService(IRepositoryUoW repositoryUoW)
        {
            _repositoryUoW = repositoryUoW;
        }

        public async Task<Result<UserEntity>> Add(UserEntity userEntity)
        {
            using var transaction = _repositoryUoW.BeginTransaction();

            try
            {
                var isValid = await IsValidUserRequest(userEntity);
                if (!isValid.Success)
                    return Result<UserEntity>.Error(isValid.Message);

                await _repositoryUoW.UserRepository.Add(userEntity);
                await _repositoryUoW.SaveAsync();
                await transaction.CommitAsync();

                Log.Information(LogMessages.AddingUserSuccess());
                return Result<UserEntity>.Ok();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Log.Error(LogMessages.AddingUserError(ex));
                return Result<UserEntity>.Error($"Error to add a new User: {ex.Message}");
            }
        }

        public async Task<Result<UserEntity>> Update(UserEntity userEntity)
        {
            using var transaction = _repositoryUoW.BeginTransaction();

            try
            {
                var userById = await GetExistingUserOrThrowAsync(userEntity.Id, "update");
                userById.Email = userEntity.Email;
                userById.Name = userEntity.Name;
                userById.ModificationDate = DateTime.UtcNow;

                _repositoryUoW.UserRepository.Update(userById);
                await _repositoryUoW.SaveAsync();
                await transaction.CommitAsync();

                Log.Information(LogMessages.UpdatingSuccessUser());
                return Result<UserEntity>.Ok();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Log.Error(LogMessages.UpdatingErrorUser(ex));
                throw new InvalidOperationException($"Failed to update user with id {userEntity.Id}. See logs for details.", ex);
            }
        }

        public async Task<bool> Delete(int id)
        {
            using var transaction = _repositoryUoW.BeginTransaction();

            try
            {
                var user = await GetExistingUserOrThrowAsync(id, "delete");
                user.IsActive = false;
                user.ModificationDate = DateTime.UtcNow;

                _repositoryUoW.UserRepository.Update(user);
                await _repositoryUoW.SaveAsync();
                await transaction.CommitAsync();

                Log.Information(LogMessages.DeleteUserSuccess());
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Log.Error(LogMessages.DeleteUserError(ex));
                throw new InvalidOperationException($"Failed to delete user with id {id}. See logs for details.", ex);
            }
        }

        public async Task<List<UserEntity>> Get()
        {
            using var transaction = _repositoryUoW.BeginTransaction();

            try
            {
                List<UserEntity> userEntities = await _repositoryUoW.UserRepository.Get();
                _repositoryUoW.Commit();

                Log.Information(LogMessages.GetAllUserSuccess());
                return userEntities;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Log.Error(LogMessages.GetAllUserError(ex));
                throw new InvalidOperationException("Error to loading the list User. See logs for details.", ex);
            }
        }

        public async Task<Result<UserEntity>> GetById(int id)
        {
            throw new NotImplementedException();
        }        

        private async Task<Result<UserEntity>> IsValidUserRequest(UserEntity userEntity)
        {
            var requestValidator = await new UserRequestValidator().ValidateAsync(userEntity);

            if (!requestValidator.IsValid)
            {
                string errorMessage = string.Join(" ", requestValidator.Errors.Select(e => e.ErrorMessage));
                errorMessage = errorMessage.Replace(Environment.NewLine, "");
                return Result<UserEntity>.Error(errorMessage);
            }

            return Result<UserEntity>.Ok();
        }

        private async Task<UserEntity> GetExistingUserOrThrowAsync(int userId, string action)
        {
            var user = await _repositoryUoW.UserRepository.GetById(userId);

            if (user is null)
            {
                var message = LogMessages.CannotPerformActionOnUser(action, userId);
                Log.Error(message);
                throw new InvalidOperationException(message);
            }

            return user;
        }
    }
}