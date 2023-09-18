using Blog.PublicAPI.Domain.PostAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.PublicAPI.Data.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder
            .HasKey(tag => tag.Id);

        builder
            .Property(tag => tag.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder
            .Property(tag => tag.PostId)
            .IsRequired();

        builder
            .Property(tag => tag.Title)
            .IsRequired()
            .HasMaxLength(30);

        builder
            .HasIndex(tag => new { tag.PostId, tag.Title })
            .IsUnique();
    }
}
