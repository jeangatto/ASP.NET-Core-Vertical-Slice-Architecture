using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using AutoMapper;
using Blog.PublicAPI.Data;
using Blog.PublicAPI.Domain.PostAggregate;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Blog.PublicAPI.Features.Posts;

public class CreatePostRequestHandler : IRequestHandler<CreatePostRequest, Result<PostResponse>>
{
    private readonly BlogContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    private readonly IValidator<CreatePostRequest> _validator;

    public CreatePostRequestHandler(
        BlogContext context,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper,
        IValidator<CreatePostRequest> validator)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<PostResponse>> Handle(CreatePostRequest request, CancellationToken cancellationToken)
    {
        var result = await _validator.ValidateAsync(request, cancellationToken);
        if (!result.IsValid)
        {
            return Result<PostResponse>.Invalid(result.AsErrors());
        }

        if (await _context.Posts.AsNoTracking().AnyAsync(post => post.Title == request.Title, cancellationToken))
        {
            return Result<PostResponse>.Conflict("There is already a post with the given title.");
        }

        var claim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);

        var userId = Guid.Parse(claim.Value);

        var post = Post.Create(userId, request.Title, request.Content, request.Tags);

        _context.Add(post);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(_mapper.Map<PostResponse>(post));
    }
}