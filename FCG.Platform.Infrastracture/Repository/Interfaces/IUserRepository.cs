using FCG.Platform.Domain.Entities.Dto.UserDto;
using FCG.Platform.Domain.Entities.Entity;

namespace FCG.Platform.Domain.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<UserEntity> Add(UserEntity userEntity);
        UserEntity Update(UserEntity userEntity);
        Task<bool> Delete(string id);
        Task<List<UserResponse>> Get();
        Task<UserEntity?> GetByIdCheck(string id);
        Task<bool> CheckPassword(UserEntity userEntity, string password);
        Task<UserEntity> GetByEmail(string email);
    }
}