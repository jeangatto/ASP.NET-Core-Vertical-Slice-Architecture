using System;
using System.Threading.Tasks;
using Blog.PublicAPI.Data;
using Blog.PublicAPI.Domain.UserAggregate;
using Microsoft.EntityFrameworkCore;

namespace Blog.PublicAPI.Features.Users;

public class UserRepository : EfRepositoryBase<User>, IUserRepository
{
    public UserRepository(BlogContext dbContext) : base(dbContext)
    {
    }

    public async Task<User> GetByEmailAsync(string email) =>
        await DbSet.AsNoTracking().FirstAsync(user => user.Email == email);

    public async Task<bool> ExistsAsync(string email) =>
        await DbSet.AsNoTracking().AnyAsync(user => user.Email == email);

    public async Task<bool> ExistsAsync(string email, Guid existingId) =>
        await DbSet.AsNoTracking().AnyAsync(user => user.Email == email && user.Id != existingId);
}