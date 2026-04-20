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
            throw new NotImplementedException();
        }

        public async Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<UserEntity>> Get()
        {
            throw new NotImplementedException();
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
    }
}