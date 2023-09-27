using Blog.PublicAPI.Domain.PostAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.PublicAPI.Data.Configurations;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder
            .HasKey(author => author.Id);

        builder
            .Property(author => author.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder
            .Property(author => author.Name)
            .IsRequired()
            .HasMaxLength(150);
    }
}