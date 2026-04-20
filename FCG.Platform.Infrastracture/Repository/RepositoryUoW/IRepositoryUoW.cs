using FCG.Platform.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace FCG.Platform.Infrastracture.Repository.RepositoryUoW
{
    public interface IRepositoryUoW
    {
        IUserRepository UserRepository { get; }
        IGameRepository GameRepository { get; }

        Task SaveAsync();
        void Commit();
        IDbContextTransaction BeginTransaction();
    }
}