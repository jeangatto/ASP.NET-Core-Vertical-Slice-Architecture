using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using Blog.PublicAPI.Data;
using Blog.PublicAPI.Domain.PostAggregate;
using FluentValidation;
using MediatR;

namespace Blog.PublicAPI.Features.Posts;

public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, Result>
{
    private readonly IValidator<CreatePostCommand> _validator;
    private readonly IPostUniquenessChecker _uniquenessChecker;
    private readonly BlogContext _dbContext;

    public CreatePostCommandHandler(
        IPostUniquenessChecker uniquenessChecker,
        IValidator<CreatePostCommand> validator,
        BlogContext dbContext)
    {
        _uniquenessChecker = uniquenessChecker;
        _validator = validator;
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return Result.Invalid(validationResult.AsErrors());

        try
        {
            var post = await Post.CreateAsync(request.Title, request.Content, request.Tags, _uniquenessChecker);

            _dbContext.Posts.Add(post);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.SuccessWithMessage("Successfully created post");
        }
        catch (PostMustBeUniqueException ex)
        {
            return Result.Error(ex.Message);
        }
    }
}
