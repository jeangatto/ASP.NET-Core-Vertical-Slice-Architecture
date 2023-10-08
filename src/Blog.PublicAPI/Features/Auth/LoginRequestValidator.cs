using FluentValidation;

namespace Blog.PublicAPI.Features.Auth;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(150);

        RuleFor(request => request.Password)
            .NotEmpty();
    }
}