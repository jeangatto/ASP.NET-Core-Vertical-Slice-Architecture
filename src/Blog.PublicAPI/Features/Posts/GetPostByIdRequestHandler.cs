using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using AutoMapper;
using Blog.PublicAPI.Domain.PostAggregate;
using MediatR;

namespace Blog.PublicAPI.Features.Posts;

public class GetPostByIdRequestHandler : IRequestHandler<GetPostByIdRequest, Result<PostResponse>>
{
    private readonly IMapper _mapper;
    private readonly IPostRepository _repository;

    public GetPostByIdRequestHandler(IMapper mapper, IPostRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<Result<PostResponse>> Handle(GetPostByIdRequest request, CancellationToken cancellationToken)
    {
        var postDomain = await _repository.GetByIdAsync(request.Id);
        if (postDomain == null)
        {
            return Result.NotFound($"No posts found by id = {request.Id}");
        }

        var response = _mapper.Map<PostResponse>(postDomain);

        return Result.Success(response);
    }
}