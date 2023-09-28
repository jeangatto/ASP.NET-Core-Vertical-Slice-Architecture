using FluentValidation;

namespace Blog.PublicAPI.Features.Users;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(request => request.UserName)
            .NotEmpty()
            .MaximumLength(20);


        RuleFor(request => request.Email)
            .NotEmpty()
            .MaximumLength(250)
            .EmailAddress();

        RuleFor(request => request.Password)
            .NotEmpty()
            .MinimumLength(4)
            .MaximumLength(16);
    }
}