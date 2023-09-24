using System.Threading.Tasks;
using Blog.PublicAPI.Data;
using Blog.PublicAPI.Domain.PostAggregate;
using Microsoft.EntityFrameworkCore;

namespace Blog.PublicAPI.Features.Posts;

public class PostUniquessCheckerService : IPostUniquenessChecker
{
    private const string ErrorMessage = "There is already a registered post with the given title";
    private readonly BlogContext _dbContext;

    public PostUniquessCheckerService(BlogContext dbContext) => _dbContext = dbContext;

    public async Task ValidateTitleIsUniqueAsync(string title)
    {
        if (await _dbContext.Posts.AsNoTracking().AnyAsync(post => post.Title == title))
            throw new PostMustBeUniqueException(ErrorMessage);
    }

    public async Task ValidateTitleIsUniqueAsync(string title, Post postBeingUpdated)
    {
        if (await _dbContext.Posts.AsNoTracking().AnyAsync(post => post.Title == title && post.Id != postBeingUpdated.Id))
            throw new PostMustBeUniqueException(ErrorMessage);
    }
}
