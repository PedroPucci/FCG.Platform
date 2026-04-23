using FCG.Platform.Domain.Entities.Dto;
using FCG.Platform.Domain.Entities.Entity;

namespace FCG.Platform.Domain.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<UserEntity> Add(UserResponse userEntity);
        UserEntity Update(UserEntity userEntity);
        Task<bool> Delete(int id);
        Task<List<UserResponse>> Get();
        Task<UserResponse?> GetById(int id);
        Task<UserEntity?> GetByIdCheck(int id);
        Task<bool> CheckPassword(UserEntity userEntity, string password);
        Task<UserEntity> GetByEmail(string email);
    }
}