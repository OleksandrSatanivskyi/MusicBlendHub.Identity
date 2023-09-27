using FluentValidation;
using MusicBlendHub.Identity.Requests;

namespace MusicBlendHub.Identity.Validations
{
    public class RegisterRequestValidator: AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(user => user.Name)
                .NotEmpty().WithMessage("Name cannot be empty")
                .MinimumLength(4).WithMessage("Username must be at least 4 characters long")
                .MaximumLength(16).WithMessage("Username must be at most 16 characters long");

            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Email cannot be empty")
                .EmailAddress().WithMessage("Invalid Email format");

            RuleFor(user => user.Password)
                .NotEmpty().WithMessage("Password cannot be empty")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
                .MaximumLength(256).WithMessage("Password must be at most 16 characters long")
                .Matches("[A-Za-z]").WithMessage("Password must contain at least one letter")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit");

            RuleFor(user => user.ConfirmPassword)
                .Equal(request => request.Password).WithMessage("Passwords do not match");

        }
    }
}
