using FluentValidation;

namespace Blog.PublicAPI.Features.Posts;

public class UpdatePostCommandValidator : AbstractValidator<UpdatePostCommand>
{
    public UpdatePostCommandValidator()
    {
        RuleFor(command => command.Title)
            .NotEmpty();

        RuleFor(command => command.Content)
            .NotEmpty();

        RuleFor(command => command.Tags)
            .NotEmpty();
    }
}
