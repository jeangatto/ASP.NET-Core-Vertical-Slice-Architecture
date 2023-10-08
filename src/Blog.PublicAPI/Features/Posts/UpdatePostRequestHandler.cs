using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using AutoMapper;
using Blog.PublicAPI.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Blog.PublicAPI.Features.Posts;

public class UpdatePostRequestHandler : IRequestHandler<UpdatePostRequest, Result<PostResponse>>
{
    private readonly BlogContext _context;
    private readonly IMapper _mapper;
    private readonly IValidator<UpdatePostRequest> _validator;

    public UpdatePostRequestHandler(BlogContext context, IMapper mapper, IValidator<UpdatePostRequest> validator)
    {
        _context = context;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<PostResponse>> Handle(UpdatePostRequest request, CancellationToken cancellationToken)
    {
        var result = await _validator.ValidateAsync(request, cancellationToken);
        if (!result.IsValid)
        {
            return Result.Invalid(result.AsErrors());
        }

        if (await _context.Posts.AsNoTracking().AnyAsync(post => post.Title == request.Title && post.Id != request.Id, cancellationToken))
        {
            return Result.Conflict("There is already a post with the given title.");
        }

        var post = await _context.Posts.FindAsync(new object[] { request.Id }, cancellationToken);
        if (post == null)
        {
            return Result.NotFound($"No posts found by id = {request.Id}");
        }

        post.Update(request.Title, request.Content, request.Tags);

        _context.Update(post);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(_mapper.Map<PostResponse>(post));
    }
}