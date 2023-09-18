using Blog.PublicAPI.Data.Configurations;
using Blog.PublicAPI.Domain.PostAggregate;
using Microsoft.EntityFrameworkCore;

namespace Blog.PublicAPI.Data;

public class BlogContext : DbContext
{
    public BlogContext(DbContextOptions<BlogContext> options)
        : base(options)
    {
    }

    public DbSet<Post> Posts => Set<Post>();

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
            .ApplyConfiguration(new PostConfiguration())
            .ApplyConfiguration(new TagConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
