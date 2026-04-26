using FCG.Platform.Domain.Entities.Dto.UserDto;
using FCG.Platform.Shared.DomainErrors;
using FCG.Platform.Shared.Helpers;
using FluentValidation;

namespace FCG.Platform.Shared.Validator
{
    public class UserRequestValidator : AbstractValidator<UserResponse>
    {
        public UserRequestValidator()
        {
            RuleFor(p => p.Email)
                .NotEmpty()
                    .WithMessage(UserErrors.User_Error_EmailCanNotBeNullOrEmpty.Description())
                .Matches(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")
                    .WithMessage(UserErrors.User_Error_InvalidEmailFormat.Description());

            RuleFor(p => p.Password)
                .NotEmpty()
                    .WithMessage(UserErrors.User_Error_PasswordCanNotBeNullOrEmpty.Description())
                .MinimumLength(8)
                    .WithMessage(UserErrors.User_Error_PasswordLengthLessEight.Description())
                .Matches(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,}$")
                    .WithMessage(UserErrors.User_Error_PasswordInvalid.Description());

            RuleFor(p => p.Name)
                .NotEmpty()
                    .WithMessage(UserErrors.User_Error_NameCanNotBeNullOrEmpty.Description())
                .MinimumLength(8)
                    .WithMessage(UserErrors.User_Error_NameLengthLessEight.Description());
        }
    }
}