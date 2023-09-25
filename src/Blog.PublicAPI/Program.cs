using System.IO.Compression;
using Blog.PublicAPI.Data;
using Blog.PublicAPI.Features.Posts;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
builder.Services.Configure<KestrelServerOptions>(options => options.AddServerHeader = false);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.AddDbContext<BlogContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase(nameof(BlogContext)));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreatePostRequest>());
builder.Services.AddValidatorsFromAssemblyContaining<CreatePostRequestValidator>();

builder.WebHost.UseKestrel(options => options.AddServerHeader = false);

builder.Host.UseDefaultServiceProvider((context, serviceProviderOptions) =>
{
    serviceProviderOptions.ValidateScopes = context.HostingEnvironment.IsDevelopment();
    serviceProviderOptions.ValidateOnBuild = true;
});

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
