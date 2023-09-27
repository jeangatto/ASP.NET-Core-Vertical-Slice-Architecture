using System;
using System.Threading.Tasks;
using Blog.PublicAPI.Data;
using Blog.PublicAPI.Domain.PostAggregate;
using Microsoft.EntityFrameworkCore;

namespace Blog.PublicAPI.Features.Posts;

public class PostRepository : IPostRepository
{
    private readonly BlogContext _context;

    public PostRepository(BlogContext context) => _context = context;

    public async Task AddAsync(Post post)
    {
        _context.Add(post);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Post post)
    {
        _context.Update(post);
        await _context.SaveChangesAsync();
    }

    public async Task<Post> GetByIdAsync(Guid id)
    {
        return await _context.Posts
            .AsNoTrackingWithIdentityResolution()
            .Include(post => post.Tags)
            .FirstOrDefaultAsync(post => post.Id == id);
    }

    public async Task<bool> ExistsAsync(string title)
    {
        return await _context.Posts
            .AsNoTracking()
            .AnyAsync(post => post.Title == title);
    }

    public async Task<bool> ExistsAsync(string title, Guid existingId)
    {
        return await _context.Posts
            .AsNoTracking()
            .AnyAsync(post => post.Title == title && post.Id != existingId);
    }
}