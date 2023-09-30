using System;
using System.Threading.Tasks;
using Blog.PublicAPI.Data;
using Blog.PublicAPI.Domain.PostAggregate;
using Microsoft.EntityFrameworkCore;

namespace Blog.PublicAPI.Features.Posts;

public class PostRepository : EfRepositoryBase<Post>, IPostRepository
{
    public PostRepository(BlogContext dbContext) : base(dbContext)
    {
    }

    public async Task<Post> GetByIdAsync(Guid id) =>
        await DbSet.AsNoTracking().Include(post => post.Tags).FirstOrDefaultAsync(post => post.Id == id);

    public async Task<bool> ExistsAsync(string title) =>
        await DbSet.AsNoTracking().AnyAsync(post => post.Title == title);

    public async Task<bool> ExistsAsync(string title, Guid existingId) =>
        await DbSet.AsNoTracking().AnyAsync(post => post.Title == title && post.Id != existingId);
}