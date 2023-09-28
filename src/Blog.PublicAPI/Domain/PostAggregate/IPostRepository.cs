using System;
using System.Threading.Tasks;

namespace Blog.PublicAPI.Domain.PostAggregate;

public interface IPostRepository
{
    Task AddAsync(Post post);
    Task UpdateAsync(Post post);
    Task<Post> GetByIdAsync(Guid id);
    Task<bool> ExistsAsync(string title);
    Task<bool> ExistsAsync(string title, Guid existingId);
}