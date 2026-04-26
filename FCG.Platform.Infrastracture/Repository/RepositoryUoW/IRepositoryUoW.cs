using FCG.Platform.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace FCG.Platform.Infrastracture.Repository.RepositoryUoW
{
    public interface IRepositoryUoW
    {
        IUserRepository UserRepository { get; }
        IGameRepository GameRepository { get; }
        IUserGameRepository UserGameRepository { get; }

        Task SaveAsync();
        void Commit();
        IDbContextTransaction BeginTransaction();
    }
}