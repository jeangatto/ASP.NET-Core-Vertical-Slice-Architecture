using Blog.PublicAPI.Data;
using Blog.PublicAPI.Domain.PostAggregate;
using Blog.PublicAPI.Features.Posts;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<KestrelServerOptions>(options => options.AddServerHeader = false);
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddResponseCompression();

builder.Services.AddDbContext<BlogContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase(nameof(BlogContext)));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreatePostRequest>());
builder.Services.AddValidatorsFromAssemblyContaining<CreatePostRequestValidator>();
builder.Services.AddScoped<IPostRepository, PostRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();