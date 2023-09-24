using FluentValidation;

namespace Blog.PublicAPI.Features.Posts;

public class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
{
    public CreatePostCommandValidator()
    {
        RuleFor(command => command.Title)
            .NotEmpty();

        RuleFor(command => command.Content)
            .NotEmpty();

        RuleFor(command => command.Tags)
            .NotEmpty();
    }
}
