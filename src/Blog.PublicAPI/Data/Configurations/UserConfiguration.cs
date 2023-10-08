using Blog.PublicAPI.Domain.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.PublicAPI.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .HasKey(user => user.Id);

        builder
            .Property(user => user.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder
            .Property(user => user.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder
            .Property(user => user.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder
            .Property(user => user.HashedPassword)
            .IsRequired()
            .HasMaxLength(60);

        builder
            .Property(user => user.CreatedAt)
            .IsRequired()
            .ValueGeneratedNever();

        builder
            .HasIndex(user => user.Email)
            .IsUnique();
    }
}