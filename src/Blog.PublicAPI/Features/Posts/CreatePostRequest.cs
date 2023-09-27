using Ardalis.Result;
using MediatR;

namespace Blog.PublicAPI.Features.Posts;

public record CreatePostRequest(string Title, string Content, string[] Tags) : IRequest<Result>;