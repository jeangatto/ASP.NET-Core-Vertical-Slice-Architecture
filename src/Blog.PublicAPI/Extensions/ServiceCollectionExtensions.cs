using System;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using AutoMapper;
using Blog.PublicAPI.Data;
using Blog.PublicAPI.SharedKernel;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Blog.PublicAPI.Extensions;

public static class ServiceCollectionExtensions
{
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

    public static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(swaggerOptions =>
        {
            swaggerOptions.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Blog",
                Description = "ASP.NET Core C# Vertical Slice Architecture, CQRS, REST API, DDD, SOLID Principles"
            });

            swaggerOptions.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Standard authorisation using the Bearer scheme. Example: \"Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT"
            });

            swaggerOptions.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            swaggerOptions.ResolveConflictingActions(apiDescription => apiDescription.FirstOrDefault());

            // Set the comments path for the Swagger JSON and UI.
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            swaggerOptions.IncludeXmlComments(xmlPath, true);
        });
    }

    public static void AddBlogDbContext(this IServiceCollection services)
    {
        services.AddSingleton<DbConnection>(_ =>
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            return connection;
        });

        services.AddDbContext<BlogDbContext>((serviceProvider, optionsBuilder) =>
        {
            var connection = serviceProvider.GetRequiredService<DbConnection>();
            optionsBuilder.UseSqlite(connection);
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