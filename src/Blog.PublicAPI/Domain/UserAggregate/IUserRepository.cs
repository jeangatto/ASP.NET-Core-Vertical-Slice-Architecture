using System;
using System.Threading.Tasks;

namespace Blog.PublicAPI.Domain.UserAggregate;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task<User> GetByEmailAsync(string email);
    Task<bool> ExistsAsync(string email);
    Task<bool> ExistsAsync(string email, Guid existingId);
}