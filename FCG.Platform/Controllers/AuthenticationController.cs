//using FCG.Platform.Application.UnitOfWork;
//using FCG.Platform.Domain.Entities.Dto;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace FCG.Platform.Controllers
//{
//    [ApiController]
//    [Route("api/auth")]
//    public class AuthenticationController : Controller
//    {

//        private readonly IUnitOfWorkService _uow;

//        public AuthenticationController(IUnitOfWorkService uow)
//        {
//            _uow = uow;
//        }

//        [HttpPost("login")]
//        [AllowAnonymous]
//        public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDTO user)
//        {
//            var result = await _uow.UserService.Login(user);
//            return Ok(result);
//        }
//    }
//}

using FCG.Platform.Application.UnitOfWork;
using FCG.Platform.Domain.Entities.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Platform.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUnitOfWorkService _uow;

        public AuthenticationController(IUnitOfWorkService uow)
        {
            _uow = uow;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDTO user)
        {
            var result = await _uow.UserService.Login(user);
            return Ok(result);
        }
    }
}