using System;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using AutoMapper;
using Blog.PublicAPI.Data;
using Blog.PublicAPI.Shared;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Blog.PublicAPI.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    private const string ConnectionString = "DataSource=:memory:";
    private const string SecurityScheme = "Bearer";

    public static void ConfigureJwtBearer(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = configuration
            .GetRequiredSection("Jwt")
            .Get<JwtOptions>(binderOptions => binderOptions.BindNonPublicProperties = true);

        services.AddSingleton(jwtOptions);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwtBearerOptions =>
            {
                var signingKeyBytes = Encoding.UTF8.GetBytes(jwtOptions.SigningKey);

                jwtBearerOptions.SaveToken = true;
                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes)
                };
            });

        services.AddAuthorization();
    }

    public static void AddBlogDbContext(this IServiceCollection services)
    {
        services.AddSingleton<DbConnection>(_ =>
        {
            var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            return connection;
        });

        services.AddDbContext<BlogDbContext>((serviceProvider, optionsBuilder) =>
            optionsBuilder.UseSqlite(serviceProvider.GetRequiredService<DbConnection>()));
    }

    public static void AddFeatures(this IServiceCollection services)
    {
        var assemblyToScan = typeof(Program).Assembly;
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assemblyToScan));
        services.AddValidatorsFromAssembly(assemblyToScan);
        services.AddSingleton<IMapper>(new Mapper(new MapperConfiguration(cfg => cfg.AddMaps(assemblyToScan))));
    }
}