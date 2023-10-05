using System.Data.Common;
using AutoMapper;
using Blog.PublicAPI.Data;
using FluentValidation;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.PublicAPI.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddBlogContext(this IServiceCollection services)
    {
        services.AddSingleton<DbConnection>(_ =>
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            return connection;
        });

        services.AddDbContext<BlogContext>((serviceProvider, optionsBuilder) =>
        {
            var connection = serviceProvider.GetRequiredService<DbConnection>();
            optionsBuilder.UseSqlite(connection);
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution);
        });
    }

    public static void AddFeatures(this IServiceCollection services)
    {
        var assemblyToScan = typeof(Program).Assembly;
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assemblyToScan));
        services.AddValidatorsFromAssembly(assemblyToScan);
        services.AddSingleton<IMapper>(new Mapper(new MapperConfiguration(cfg => cfg.AddMaps(assemblyToScan))));
    }
}