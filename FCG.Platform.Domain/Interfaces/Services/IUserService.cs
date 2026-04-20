using FCG.Platform.Domain.Entities.Entity;
using FCG.Platform.Domain.OperationResult;

namespace FCG.Platform.Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task<Result<UserEntity>> Add(UserEntity userEntity);
        Task<Result<UserEntity>> Update(UserEntity userEntity);
        Task<bool> Delete(int id);
        Task<List<UserEntity>> Get();
        Task<Result<UserEntity>> GetById(int id);
    }
}