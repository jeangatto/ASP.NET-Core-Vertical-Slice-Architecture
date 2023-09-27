using Blog.PublicAPI.Domain.PostAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.PublicAPI.Data.Configurations;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder
            .HasKey(post => post.Id);

        builder
            .Property(post => post.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder
            .Property(post => post.AuthorId)
            .IsRequired();

        builder
            .Property(post => post.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .Property(post => post.Content)
            .IsRequired();

        builder
            .Property(post => post.CreatedAt)
            .IsRequired();

        builder
            .Property(post => post.UpdatedAt)
            .IsRequired(false);

        builder
            .HasOne(post => post.Author)
            .WithMany()
            .HasForeignKey(post => post.AuthorId)
            .IsRequired();

        builder
            .HasMany(post => post.Tags)
            .WithOne()
            .HasForeignKey(tag => tag.PostId)
            .IsRequired();
    }
}