using FCG.Platform.Domain.Entities.Dto;
using FCG.Platform.Domain.Entities.Entity;
using FCG.Platform.Domain.OperationResult;

namespace FCG.Platform.Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task<Result<UserEntity>> Add(UserEntity userEntity);
        Task<Result<UserEntity>> Update(UserEntity userEntity);
        Task<bool> Delete(int id);
        Task<List<UserResponse>> Get();
        Task<Result<UserResponse>> GetById(int id);
    }
}