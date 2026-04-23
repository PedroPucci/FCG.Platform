using FCG.Platform.Application.Services;
using FCG.Platform.Domain.Interfaces.Services;

namespace FCG.Platform.Application.UnitOfWork
{
    public interface IUnitOfWorkService
    {
        UserService UserService { get; }
        GameService GameService { get; }
    }
}