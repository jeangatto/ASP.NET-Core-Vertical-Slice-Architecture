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

public class CreatePostRequestHandler : IRequestHandler<CreatePostRequest, Result>
{
    private readonly IValidator<CreatePostRequest> _validator;
    private readonly BlogContext _dbContext;

    public CreatePostRequestHandler(IValidator<CreatePostRequest> validator, BlogContext dbContext)
    {
        _validator = validator;
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(CreatePostRequest request, CancellationToken cancellationToken)
    {
        var result = await _validator.ValidateAsync(request, cancellationToken);
        if (!result.IsValid)
        {
            return Result.Invalid(result.AsErrors());
        }

        if (await _dbContext.Posts.AsNoTracking()
            .AnyAsync(post => post.Title == request.Title, cancellationToken))
        {
            return Result.Error("There is already a registered post with the given title");
        }

        var post = Post.Create(request.Title, request.Content, request.Tags);

        _dbContext.Posts.Add(post);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
