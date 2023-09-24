using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using Blog.PublicAPI.Data;
using Blog.PublicAPI.Domain.PostAggregate;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Blog.PublicAPI.Features.Posts;

public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, Result>
{
    private readonly IValidator<CreatePostCommand> _validator;
    private readonly BlogContext _dbContext;

    public CreatePostCommandHandler(IValidator<CreatePostCommand> validator, BlogContext dbContext)
    {
        _validator = validator;
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var result = await _validator.ValidateAsync(request, cancellationToken);
        if (!result.IsValid)
        {
            return Result.Invalid(result.AsErrors());
        }

        if (await _dbContext.Posts.AsNoTracking().AnyAsync(post => post.Title == request.Title, cancellationToken))
        {
            return Result.Error("There is already a registered post with the given title");
        }

        var post = Post.Create(request.Title, request.Content, request.Tags);

        _dbContext.Posts.Add(post);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
