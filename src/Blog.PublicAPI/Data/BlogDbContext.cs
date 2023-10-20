using System.Reflection;
using Blog.PublicAPI.Data.Extensions;
using Blog.PublicAPI.Domain.PostAggregate;
using Blog.PublicAPI.Domain.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Blog.PublicAPI.Data;

public class BlogDbContext : DbContext
{
    public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
    {
    }

    public DbSet<Post> Posts => Set<Post>();
    public DbSet<User> Users => Set<User>();

    public override ChangeTracker ChangeTracker
    {
        get
        {
            base.ChangeTracker.LazyLoadingEnabled = false;
            return base.ChangeTracker;
        }
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<string>()
            .AreUnicode(false)
            .HaveMaxLength(250);

        base.ConfigureConventions(configurationBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly())
            .RemoveCascadeDeleteConvention();

        base.OnModelCreating(modelBuilder);
    }
}