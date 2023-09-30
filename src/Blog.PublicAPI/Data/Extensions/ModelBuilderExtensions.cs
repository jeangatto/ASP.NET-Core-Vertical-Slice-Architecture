using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Blog.PublicAPI.Data.Extensions;

public static class ModelBuilderExtensions
{
    public static ModelBuilder RemoveCascadeDeleteConvention(this ModelBuilder modelBuilder)
    {
        // Get all the foreign keys in the model that are not ownership and have cascade delete behavior
        var foreignKeys = modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(entity => entity.GetForeignKeys())
            .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade)
            .ToList();

        // Change the delete behavior of each foreign key to restrict
        foreach (var fk in foreignKeys)
            fk.DeleteBehavior = DeleteBehavior.Restrict;

        return modelBuilder;
    }
}