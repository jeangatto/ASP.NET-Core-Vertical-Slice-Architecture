using FluentValidation;

namespace Blog.PublicAPI.Features.Posts;

public class UpdatePostRequestValidator : AbstractValidator<UpdatePostRequest>
{
    public UpdatePostRequestValidator()
    {
        RuleFor(request => request.Title)
            .NotEmpty();

        RuleFor(request => request.Content)
            .NotEmpty();

        RuleFor(request => request.Tags)
            .NotEmpty();
    }
}