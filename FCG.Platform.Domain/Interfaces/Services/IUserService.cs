using FCG.Platform.Domain.Entities.Entity;

namespace FCG.Platform.Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserEntity> Add(UserEntity userEntity);
        Task<UserEntity> Update(UserEntity userEntity);
        Task<bool> Delete(int id);
        Task<List<UserEntity>> Get();
        Task<UserEntity> GetById(int id);
    }
}