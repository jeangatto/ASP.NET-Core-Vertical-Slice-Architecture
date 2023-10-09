using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using AutoMapper;
using Blog.PublicAPI.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Blog.PublicAPI.Features.Posts;

public class GetPostByIdRequestHandler : IRequestHandler<GetPostByIdRequest, Result<PostResponse>>
{
    private readonly BlogContext _context;
    private readonly IMapper _mapper;

    public GetPostByIdRequestHandler(BlogContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<PostResponse>> Handle(GetPostByIdRequest request, CancellationToken cancellationToken)
    {
        var post = await _context.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(post => post.Id == request.Id, cancellationToken);

        return post == null
            ? Result<PostResponse>.NotFound($"No posts found by id = {request.Id}")
            : Result<PostResponse>.Success(_mapper.Map<PostResponse>(post));
    }
}