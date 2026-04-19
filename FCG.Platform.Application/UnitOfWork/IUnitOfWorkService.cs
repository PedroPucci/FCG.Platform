using FCG.Platform.Application.Services;

namespace FCG.Platform.Application.UnitOfWork
{
    public interface IUnitOfWorkService
    {
        UserService UserService { get; }
    }
}