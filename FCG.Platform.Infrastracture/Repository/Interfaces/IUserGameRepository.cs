using FCG.Platform.Domain.Entities.Entity;

namespace FCG.Platform.Infrastracture.Repository.RepositoryUoW
{
    public interface IUserGameRepository
    {
        Task<UserGameEntity> Add(UserGameEntity userGameEntity);
    }
}