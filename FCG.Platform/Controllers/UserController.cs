using FCG.Platform.Application.UnitOfWork;
using FCG.Platform.Domain.Entities.Dto.UserDto;
using FCG.Platform.Domain.Entities.Entity;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Platform.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWorkService _uow;

        public UserController(IUnitOfWorkService uow)
        {
            _uow = uow;
        }

        [HttpPost()]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Add([FromBody] UserResponse userEntity)
        {
            var result = await _uow.UserService.Add(userEntity);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] UpdateUserRequest updateUserRequest)
        {
            var result = await _uow.UserService.Update(id, updateUserRequest);

            if (!result.Success)
                return NotFound(result);

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            var result = await _uow.UserService.Delete(id);

            if (!result.Success)
                return NotFound(result);

            return NoContent();
        }

        [HttpGet("all")]
        [ProducesResponseType(typeof(List<UserEntity>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get()
        {
            var result = await _uow.UserService.Get();
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(UserEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] string id)
        {
            var result = await _uow.UserService.GetById(id);
            return Ok(result);
        }
    }
}