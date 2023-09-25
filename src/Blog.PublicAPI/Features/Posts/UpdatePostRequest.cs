using System;
using Ardalis.Result;
using MediatR;

namespace Blog.PublicAPI.Features.Posts;

public record UpdatePostRequest(Guid Id, string Title, string Content, string[] Tags) : IRequest<Result>;