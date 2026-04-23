using FCG.Platform.Domain.Entities.Dto;
using FCG.Platform.Domain.Entities.Entity;
using FCG.Platform.Domain.OperationResult;

namespace FCG.Platform.Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task<Result<UserEntity>> Add(UserResponse userEntity);
        Task<Result<bool>> Update(int id, UpdateUserRequest updateUserRequest);
        Task<Result<bool>> Delete(int id);
        Task<List<UserResponse>> Get();
        Task<Result<UserResponse>> GetById(int id);
        Task<Result<string>> Login(UserForAuthenticationDTO userEntity);
    }
}