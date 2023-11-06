using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using Blog.PublicAPI.Data;
using Blog.PublicAPI.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<KestrelServerOptions>(kestrelServerOptions => kestrelServerOptions.AddServerHeader = false);
builder.Services.Configure<RouteOptions>(routeOptions => routeOptions.LowercaseUrls = true);
builder.Services.Configure<ApiBehaviorOptions>(apiBehaviorOptions => apiBehaviorOptions.SuppressModelStateInvalidFilter = true);
builder.Services.Configure<JsonOptions>(jsonOptions =>
{
    jsonOptions.JsonSerializerOptions.WriteIndented = false;
    jsonOptions.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    jsonOptions.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
    jsonOptions.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    jsonOptions.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddResponseCompression();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddProblemDetails();
builder.Services.AddControllers();

builder.Host.UseDefaultServiceProvider((context, serviceProviderOptions) =>
{
    serviceProviderOptions.ValidateScopes = context.HostingEnvironment.IsDevelopment();
    serviceProviderOptions.ValidateOnBuild = true;
});

// Application Services
builder.Services.ConfigureJwtBearer(builder.Configuration);
builder.Services.ConfigureSwagger();
builder.Services.AddBlogDbContext();
builder.Services.AddFeatures();

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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await using var serviceScope = app.Services.CreateAsyncScope();
await using var dbContext = serviceScope.ServiceProvider.GetRequiredService<BlogDbContext>();
await dbContext.Database.EnsureCreatedAsync();

var mapper = serviceScope.ServiceProvider.GetRequiredService<IMapper>();
mapper.ConfigurationProvider.AssertConfigurationIsValid();
mapper.ConfigurationProvider.CompileMappings();

app.Run();