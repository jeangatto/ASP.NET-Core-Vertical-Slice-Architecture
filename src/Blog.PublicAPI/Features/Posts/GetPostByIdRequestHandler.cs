using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Blog.PublicAPI.Domain.PostAggregate;
using MediatR;

namespace Blog.PublicAPI.Features.Posts;

public class GetPostByIdRequestHandler : IRequestHandler<GetPostByIdRequest, Result<PostResponse>>
{
    private readonly IPostRepository _repository;

    public GetPostByIdRequestHandler(IPostRepository repository) => _repository = repository;

    public async Task<Result<PostResponse>> Handle(GetPostByIdRequest request, CancellationToken cancellationToken)
    {
        var post = await _repository.GetByIdAsync(request.Id);
        if (post == null)
        {
            return Result.NotFound($"No posts found by id = {request.Id}");
        }

        var response = new PostResponse(
            post.Id,
            post.Title,
            post.Content,
            post.CreatedAt,
            post.UpdatedAt,
            post.Tags.Select(tag => $"#{tag.Title}").ToList());

        return Result.Success(response);
    }
}