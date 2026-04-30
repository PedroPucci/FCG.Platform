using FCG.Platform.Domain.Entities.Dto.UserDto;
using FCG.Platform.Domain.Entities.Entity;
using FCG.Platform.Domain.OperationResult;

namespace FCG.Platform.Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task<Result<UserEntity>> Add(UserResponse userResponse);
        Task<Result<bool>> Update(string id, UpdateUserRequest updateUserRequest);
        Task<Result<bool>> Delete(string id);
        Task<List<UserResponse>> Get();
        Task<Result<UserResponse>> GetById(string id);
    }
}