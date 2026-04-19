using FCG.Platform.Domain.Entities.Entity;
using FCG.Platform.Domain.Interfaces.Services;
using FCG.Platform.Infrastracture.Repository.RepositoryUoW;

namespace FCG.Platform.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IRepositoryUoW _repositoryUoW;

        public UserService(IRepositoryUoW repositoryUoW)
        {
            _repositoryUoW = repositoryUoW;
        }

        public Task<UserEntity> Add(UserEntity userEntity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<UserEntity>> Get()
        {
            throw new NotImplementedException();
        }

        public Task<UserEntity> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<UserEntity> Update(UserEntity userEntity)
        {
            throw new NotImplementedException();
        }
    }
}