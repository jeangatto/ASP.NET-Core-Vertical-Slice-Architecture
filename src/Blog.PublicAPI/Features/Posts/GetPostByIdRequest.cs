using System;
using Ardalis.Result;
using MediatR;

namespace Blog.PublicAPI.Features.Posts;

public record GetPostByIdRequest(Guid Id) : IRequest<Result<PostResponse>>;