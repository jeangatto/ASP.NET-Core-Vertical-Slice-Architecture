using System;
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

public class CreatePostRequestHandler(
    BlogDbContext dbContext,
    IHttpContextAccessor httpContextAccessor,
    IMapper mapper,
    IValidator<CreatePostRequest> validator) : IRequestHandler<CreatePostRequest, Result<PostResponse>>
{
    private readonly BlogDbContext _dbContext = dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IMapper _mapper = mapper;
    private readonly IValidator<CreatePostRequest> _validator = validator;

    public async Task<Result<PostResponse>> Handle(CreatePostRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<PostResponse>.Invalid(validationResult.AsErrors());
        }

        if (await _dbContext.Posts.AsNoTracking().AnyAsync(post => post.Title == request.Title, cancellationToken))
        {
            return Result<PostResponse>.Conflict("There is already a post with the given title.");
        }

        var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        var userId = Guid.Parse(claim.Value);

        var post = new Post(userId, request.Title, request.Content, request.Tags);

        _dbContext.Add(post);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(_mapper.Map<PostResponse>(post));
    }
}