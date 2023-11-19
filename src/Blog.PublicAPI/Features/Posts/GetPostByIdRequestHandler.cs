using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using AutoMapper;
using Blog.PublicAPI.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Blog.PublicAPI.Features.Posts;

public class GetPostByIdRequestHandler(BlogDbContext dbContext, IMapper mapper) : IRequestHandler<GetPostByIdRequest, Result<PostResponse>>
{
    private readonly BlogDbContext _dbContext = dbContext;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<PostResponse>> Handle(GetPostByIdRequest request, CancellationToken cancellationToken)
    {
        var post = await _dbContext.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(post => post.Id == request.Id, cancellationToken);

        return post == null
            ? Result<PostResponse>.NotFound($"No posts found by id = {request.Id}")
            : Result<PostResponse>.Success(_mapper.Map<PostResponse>(post));
    }
}