using Ardalis.Result;
using MediatR;

namespace Blog.PublicAPI.Features.Posts;

public record CreatePostCommand(string Title, string Content, string[] Tags) : IRequest<Result>;
