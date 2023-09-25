using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using Blog.PublicAPI.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Blog.PublicAPI.Features.Posts;

public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand, Result>
{
    private readonly IValidator<UpdatePostCommand> _validator;
    private readonly BlogContext _dbContext;

    public UpdatePostCommandHandler(IValidator<UpdatePostCommand> validator, BlogContext dbContext)
    {
        _validator = validator;
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var result = await _validator.ValidateAsync(request, cancellationToken);
        if (!result.IsValid)
        {
            return Result.Invalid(result.AsErrors());
        }

        if (await _dbContext.Posts.AsNoTracking()
            .AnyAsync(post => post.Title == request.Title && post.Id != request.Id, cancellationToken))
        {
            return Result.Error("There is already a registered post with the given title");
        }

        var post = await _dbContext.Posts
            .Include(post => post.Tags)
            .FirstOrDefaultAsync(post => post.Id == request.Id, cancellationToken: cancellationToken);

        if (post == null)
        {
            return Result.NotFound($"No posts found by id = {request.Id}");
        }

        post.Update(request.Title, request.Content, request.Tags);

        _dbContext.Posts.Update(post);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
