using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using AutoMapper;
using Blog.PublicAPI.Data;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Blog.PublicAPI.Features.Posts;

public class UpdatePostRequestHandler : IRequestHandler<UpdatePostRequest, Result<PostResponse>>
{
    private readonly BlogDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdatePostRequest> _validator;

    public UpdatePostRequestHandler(
        BlogDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper,
        IValidator<UpdatePostRequest> validator)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<PostResponse>> Handle(UpdatePostRequest request, CancellationToken cancellationToken)
    {
        if (!_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
        {
            return Result<PostResponse>.Unauthorized();
        }

        if (!_httpContextAccessor.HttpContext.User.HasClaim(claim => claim.Type == ClaimTypes.NameIdentifier))
        {
            return Result<PostResponse>.Forbidden();
        }

        var result = await _validator.ValidateAsync(request, cancellationToken);
        if (!result.IsValid)
        {
            return Result<PostResponse>.Invalid(result.AsErrors());
        }

        if (await _dbContext.Posts
            .AsNoTracking()
            .AnyAsync(post => post.Title == request.Title && post.Id != request.Id, cancellationToken))
        {
            return Result.Conflict("There is already a post with the given title.");
        }

        var claim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        var userId = Guid.Parse(claim.Value);

        var post = await _dbContext.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(post => post.Id == request.Id && post.AuthorId == userId, cancellationToken);

        if (post == null)
        {
            return Result<PostResponse>.NotFound($"No posts found by id = {request.Id}");
        }

        post.Update(request.Title, request.Content, request.Tags);

        _dbContext.Update(post);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(_mapper.Map<PostResponse>(post));
    }
}