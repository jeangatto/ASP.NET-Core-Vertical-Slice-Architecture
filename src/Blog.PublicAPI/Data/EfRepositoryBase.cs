using System.Threading.Tasks;
using Blog.PublicAPI.Domain;
using Microsoft.EntityFrameworkCore;

namespace Blog.PublicAPI.Data;

public abstract class EfRepositoryBase<TEntity> where TEntity : class, IAggregateRoot
{
    private readonly BlogContext _dbContext;

    protected EfRepositoryBase(BlogContext dbContext)
    {
        _dbContext = dbContext;
        DbSet = dbContext.Set<TEntity>();
    }

    protected DbSet<TEntity> DbSet { get; private init; }

    public async Task AddAsync(TEntity entity)
    {
        DbSet.Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(TEntity entity)
    {
        DbSet.Update(entity);
        await _dbContext.SaveChangesAsync();
    }
}