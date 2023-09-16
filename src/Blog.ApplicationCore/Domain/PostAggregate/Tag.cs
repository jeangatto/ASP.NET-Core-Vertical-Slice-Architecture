using System;

namespace Blog.ApplicationCore.Domain.PostAggregate;

public sealed class Tag
{
    private Tag() { }

    public Guid Id { get; private init; }
    public Guid PostId { get; private init; }
    public string Title { get; private init; }

    public static Tag Create(Guid postId, string title)
    {
        return new Tag
        {
            Id = Guid.NewGuid(),
            PostId = postId,
            Title = title
        };
    }
}