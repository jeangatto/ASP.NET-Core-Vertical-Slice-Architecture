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

    public async Task AddAsync(Post post)
    {
        DbSet.Add(post);
        await SaveChangesAsync();
    }

    public async Task UpdateAsync(Post post)
    {
        DbSet.Update(post);
        await SaveChangesAsync();
    }

    public async Task<Post> GetByIdAsync(Guid id)
    {
        return await DbSet
            .AsNoTrackingWithIdentityResolution()
            .Include(post => post.Tags)
            .FirstOrDefaultAsync(post => post.Id == id);
    }

    public async Task<bool> ExistsAsync(string title)
    {
        return await DbSet
            .AsNoTracking()
            .AnyAsync(post => post.Title == title);
    }

    public async Task<bool> ExistsAsync(string title, Guid existingId)
    {
        return await DbSet
            .AsNoTracking()
            .AnyAsync(post => post.Title == title && post.Id != existingId);
    }
}