using Ardalis.Result;
using MediatR;

namespace Blog.PublicAPI.Features.Posts;

public class CreatePostRequest : IRequest<Result<PostResponse>>
{
    public CreatePostRequest(string title, string content, string[] tags)
    {
        Title = title;
        Content = content;
        Tags = tags;
    }

    public CreatePostRequest()
    {
    }

    public string Title { get; init; }
    public string Content { get; init; }
    public string[] Tags { get; init; }
}