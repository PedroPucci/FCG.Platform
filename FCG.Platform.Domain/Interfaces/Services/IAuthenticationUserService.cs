using FCG.Platform.Domain.Entities.Dto;
using FCG.Platform.Domain.OperationResult;

namespace FCG.Platform.Domain.Interfaces.Services
{
    public interface IAuthenticationUserService
    {
        Task<Result<string>> Login(UserForAuthenticationDTO userEntity);
    }
}