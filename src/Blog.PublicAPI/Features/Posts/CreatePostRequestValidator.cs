using FluentValidation;

namespace Blog.PublicAPI.Features.Posts;

public class CreatePostRequestValidator : AbstractValidator<CreatePostRequest>
{
    public CreatePostRequestValidator()
    {
        RuleFor(request => request.Title)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.Content)
            .NotEmpty();

        RuleFor(request => request.Tags)
            .NotEmpty();
    }
}