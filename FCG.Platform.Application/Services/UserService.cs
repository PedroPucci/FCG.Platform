using FCG.Platform.Domain.Entities.Dto.UserDto;
using FCG.Platform.Domain.Entities.Entity;
using FCG.Platform.Domain.Interfaces.Services;
using FCG.Platform.Domain.OperationResult;
using FCG.Platform.Infrastracture.Repository.RepositoryUoW;
using FCG.Platform.Shared.Logging;
using FCG.Platform.Shared.Validator;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace FCG.Platform.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IRepositoryUoW _repositoryUoW;
        private readonly UserManager<UserEntity> _userManager;
        private readonly RoleManager<ProfileEntity> _roleManager;

        public UserService(
            IRepositoryUoW repositoryUoW,
            UserManager<UserEntity> userManager,
            RoleManager<ProfileEntity> roleManager)
        {
            _repositoryUoW = repositoryUoW;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<Result<UserEntity>> Add(UserResponse userReponse)
        {
            using var transaction = _repositoryUoW.BeginTransaction();

            try
            {
                var userEntity = new UserEntity
                {
                    Email = userReponse.Email,
                    Name = userReponse.Name,
                    UserName = userReponse.Email,
                    CreateDate = DateTime.UtcNow,
                    IsActive = true
                };

                var isValid = await IsValidUserRequest(userReponse);
                if (!isValid.Success)
                {
                    Log.Information(isValid.Message);
                    return Result<UserEntity>.Error(isValid.Message);
                }                    

                if (string.IsNullOrWhiteSpace(userReponse.Role))
                {
                    Log.Information("'Role' can not be null or empty!");
                    return Result<UserEntity>.Error("'Role' can not be null or empty!");
                }

                var role = userReponse.Role.Trim();

                var roleExists = await _roleManager.RoleExistsAsync(role);

                if (!roleExists)
                {
                    Log.Information("Invalid role.");
                    return Result<UserEntity>.Error("Invalid role. Use only: Administrator or Usuario.");
                }

                var createResult = await _userManager.CreateAsync(userEntity, userReponse.Password!);

                if (!createResult.Succeeded)
                {
                    var errors = string.Join(" ", createResult.Errors.Select(e => e.Description));
                    return Result<UserEntity>.Error(errors);
                }

                var roleResult = await _userManager.AddToRoleAsync(userEntity, role);

                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(" ", roleResult.Errors.Select(e => e.Description));
                    return Result<UserEntity>.Error(errors);
                }

                await transaction.CommitAsync();
                Log.Information(LogMessages.AddingUserSuccess(userEntity));
                return Result<UserEntity>.Ok(userEntity);
            }
            //catch (Exception ex)
            //{
            //    await transaction.RollbackAsync();
            //    Log.Error(LogMessages.AddingUserError(ex));
            //    return Result<UserEntity>.Error($"Error to add a new User: {ex.Message}");
            //}
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Log.Error(ex, "Error to add a new User");
                throw;
            }
        }

        public async Task<Result<bool>> Update(string id, UpdateUserRequest updateUserRequest)
        {
            using var transaction = _repositoryUoW.BeginTransaction();

            try
            {
                var user = await _repositoryUoW.UserRepository.GetByIdCheck(id);

                if (user is null)
                {
                    var message = LogMessages.CannotPerformActionOnUser("update", id);
                    Log.Error(message);
                    return Result<bool>.Error(message);
                }

                user.Email = updateUserRequest.Email;
                user.Name = updateUserRequest.Name;
                user.IsActive = updateUserRequest.IsActive;
                user.ModificationDate = DateTime.UtcNow;

                _repositoryUoW.UserRepository.Update(user);
                await _repositoryUoW.SaveAsync();
                await transaction.CommitAsync();

                Log.Information(LogMessages.UpdatingSuccessUser(user));
                return Result<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Log.Error(LogMessages.UpdatingErrorUser(ex));
                throw new InvalidOperationException($"Failed to update user with id. See logs for details.", ex);
            }
        }

        public async Task<Result<bool>> Delete(string id)
        {
            using var transaction = _repositoryUoW.BeginTransaction();

            try
            {
                var user = await _repositoryUoW.UserRepository.GetByIdCheck(id);

                if (user is null)
                {
                    transaction.Rollback();

                    var message = LogMessages.CannotPerformActionOnUser("retrieve", id);
                    Log.Error(message);

                    return Result<bool>.Error(message);
                }

                user.IsActive = false;
                user.ModificationDate = DateTime.UtcNow;

                _repositoryUoW.UserRepository.Update(user);
                await _repositoryUoW.SaveAsync();
                await transaction.CommitAsync();

                Log.Information(LogMessages.DeleteUserSuccess(user));
                return Result<bool>.Ok();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Log.Error(LogMessages.DeleteUserError(ex));
                throw new InvalidOperationException($"Failed to delete user with id {id}. See logs for details.", ex);
            }
        }

        public async Task<List<UserResponse>> Get()
        {
            using var transaction = _repositoryUoW.BeginTransaction();

            try
            {
                List<UserResponse> userEntities = await _repositoryUoW.UserRepository.Get();
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

        public async Task<Result<UserResponse>> GetById(string id)
        {
            using var transaction = _repositoryUoW.BeginTransaction();

            try
            {
                var user = await _repositoryUoW.UserRepository.GetByIdCheck(id);

                if (user is null)
                {
                    transaction.Rollback();

                    var message = LogMessages.CannotPerformActionOnUser("retrieve", id);
                    Log.Error(message);

                    return Result<UserResponse>.Error(message);
                }

                var userResponse = new UserResponse
                {
                    Email = user?.Email,
                    Name = user?.Name,
                    IsActive = user?.IsActive ?? false
                };

                _repositoryUoW.Commit();

                Log.Information(LogMessages.GetByUserIdSuccess(user));
                return Result<UserResponse>.Ok(userResponse);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Log.Error(LogMessages.GetByUserIdError(ex));
                throw new InvalidOperationException("Error retrieving the user. See inner exception for details.", ex);
            }
        }

        private async Task<Result<UserResponse>> IsValidUserRequest(UserResponse userResponse)
        {
            var requestValidator = await new UserRequestValidator().ValidateAsync(userResponse);

            if (!requestValidator.IsValid)
            {
                string errorMessage = string.Join(" ", requestValidator.Errors.Select(e => e.ErrorMessage));
                errorMessage = errorMessage.Replace(Environment.NewLine, "");
                return Result<UserResponse>.Error(errorMessage);
            }

            return Result<UserResponse>.Ok();
        }
    }
}