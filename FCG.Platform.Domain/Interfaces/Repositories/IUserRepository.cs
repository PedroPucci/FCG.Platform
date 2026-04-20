using FCG.Platform.Domain.Entities.Entity;

namespace FCG.Platform.Domain.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<UserEntity> Add(UserEntity userEntity);
        UserEntity Update(UserEntity userEntity);
        Task<bool> Delete(int id);
        Task<List<UserEntity>> Get();
        Task<UserEntity?> GetById(int id);
    }
}