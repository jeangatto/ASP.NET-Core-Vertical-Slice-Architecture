using System;
using System.Threading.Tasks;
using Blog.PublicAPI.Data;
using Blog.PublicAPI.Domain.PostAggregate;
using Microsoft.EntityFrameworkCore;

namespace Blog.PublicAPI.Features.Posts;

public class PostRepository : IPostRepository
{
    private readonly BlogContext _dbContext;

    public PostRepository(BlogContext dbContext) => _dbContext = dbContext;

    public async Task AddAsync(Post post)
    {
        _dbContext.Add(post);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Post post)
    {
        _dbContext.Update(post);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Post> GetByIdAsync(Guid id)
    {
        return await _dbContext.Posts
            .AsNoTrackingWithIdentityResolution()
            .Include(post => post.Tags)
            .FirstOrDefaultAsync(post => post.Id == id);
    }

    public async Task<bool> ExistsAsync(string title)
    {
        return await _dbContext.Posts
            .AsNoTracking()
            .AnyAsync(post => post.Title == title);
    }

    public async Task<bool> ExistsAsync(string title, Guid existingId)
    {
        return await _dbContext.Posts
            .AsNoTracking()
            .AnyAsync(post => post.Title == title && post.Id != existingId);
    }

    #region IDisposable

    // To detect redundant calls.
    private bool _disposed;

    // Public implementation of Dispose pattern callable by consumers.
    ~PostRepository() => Dispose(false);

    // Public implementation of Dispose pattern callable by consumers.
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // Protected implementation of Dispose pattern.
    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        // Dispose managed state (managed objects).
        if (disposing)
        {
            _dbContext.Dispose();
        }

        _disposed = true;
    }

    #endregion
}