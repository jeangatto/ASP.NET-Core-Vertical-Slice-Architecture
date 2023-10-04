using System.Data.Common;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using Blog.PublicAPI.Data;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<KestrelServerOptions>(options => options.AddServerHeader = false);
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

builder.Services.AddResponseCompression();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true)
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.WriteIndented = false;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    });

builder.Services.AddSingleton<DbConnection>(_ =>
{
    var connection = new SqliteConnection("DataSource=:memory:");
    connection.Open();
    return connection;
});

builder.Services.AddDbContext<BlogContext>((serviceProvider, optionsBuilder) =>
{
    var connection = serviceProvider.GetRequiredService<DbConnection>();
    optionsBuilder.UseSqlite(connection);
    optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution);
});

var assembliesToScan = typeof(Program).Assembly;

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembliesToScan));
builder.Services.AddValidatorsFromAssembly(assembliesToScan);
builder.Services.AddSingleton<IMapper>(new Mapper(new MapperConfiguration(cfg => cfg.AddMaps(assembliesToScan))));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseResponseCompression();
app.UseHttpsRedirection();
app.UseHttpLogging();
app.MapControllers();

using var serviceScope = app.Services.CreateScope();
using var dbContext = serviceScope.ServiceProvider.GetRequiredService<BlogContext>();
dbContext.Database.EnsureCreated();

var mapper = serviceScope.ServiceProvider.GetRequiredService<IMapper>();
mapper.ConfigurationProvider.AssertConfigurationIsValid();
mapper.ConfigurationProvider.CompileMappings();

app.Run();