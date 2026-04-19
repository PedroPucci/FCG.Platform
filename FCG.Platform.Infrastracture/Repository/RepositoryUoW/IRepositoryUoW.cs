using FCG.Platform.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace FCG.Platform.Infrastracture.Repository.RepositoryUoW
{
    public interface IRepositoryUoW
    {
        IUserRepository UserRepository { get; }

        Task SaveAsync();
        void Commit();
        IDbContextTransaction BeginTransaction();
    }
}