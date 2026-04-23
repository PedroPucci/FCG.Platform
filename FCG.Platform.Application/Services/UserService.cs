using FCG.Platform.Domain.Entities.Dto;
using FCG.Platform.Domain.Entities.Entity;
using FCG.Platform.Domain.Interfaces.Services;
using FCG.Platform.Domain.OperationResult;
using FCG.Platform.Infrastracture.Repository.RepositoryUoW;
using FCG.Platform.Shared.Logging;
using FCG.Platform.Shared.Validator;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FCG.Platform.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IRepositoryUoW _repositoryUoW;

        public UserService(IRepositoryUoW repositoryUoW)
        {
            _repositoryUoW = repositoryUoW;
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
                    IsActive = true,
                    PasswordHash = userReponse.Password
                };

                var isValid = await IsValidUserRequest(userEntity);
                if (!isValid.Success)
                    return Result<UserEntity>.Error(isValid.Message);

                userEntity.IsActive = true;

                await _repositoryUoW.UserRepository.Add(userEntity);
                await _repositoryUoW.SaveAsync();
                await transaction.CommitAsync();

                Log.Information(LogMessages.AddingUserSuccess(userEntity));
                return Result<UserEntity>.Ok();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Log.Error(LogMessages.AddingUserError(ex));
                return Result<UserEntity>.Error($"Error to add a new User: {ex.Message}");
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

        public async Task<Result<string>> Login(UserForAuthenticationDTO userEntity)
        {
            var response = await _repositoryUoW.UserRepository.GetByEmail(userEntity.Email);
            var result = await _repositoryUoW.UserRepository.CheckPassword(response, userEntity.Password);

            var token = await CreateAccessTokenAsync(response);
            return Result<string>.Ok(token);
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var tokenOptions = new JwtSecurityToken
            (
                issuer: "PedroIghor",
                audience: "https://localhost:5001",
                claims: claims,
                expires: DateTime.UtcNow.AddSeconds(3600),
                signingCredentials: signingCredentials
            );

            return tokenOptions;
        }
        private SymmetricSecurityKey JwtSecret() => new(Encoding.UTF8.GetBytes("EAA4Cf4JnqYwBP9MSZC8cHvMSvmShHZBU27qQxZBS3ORNSoIdEz3me0QHZABLNBiEWtDmVLZBVeMF8QZCd"));


        private async Task<string> CreateAccessTokenAsync(UserEntity user)
        {
            var signingCredentials = new SigningCredentials(JwtSecret(), SecurityAlgorithms.HmacSha256);
            var claims = await GetUserClaimsAsync(user);
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        private async Task<List<Claim>> GetUserClaimsAsync(UserEntity user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Id.ToString())
            };

            if (!string.IsNullOrWhiteSpace(user.Name))
                claims.Add(new Claim(ClaimTypes.GivenName, user.Name));

            return claims;
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