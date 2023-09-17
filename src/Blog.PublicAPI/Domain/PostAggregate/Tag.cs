using System;

namespace Blog.PublicAPI.Domain.PostAggregate;

public class Tag
{
    public Tag(Guid postId, string title)
    {
        Id = Guid.NewGuid();
        PostId = postId;
        Title = title;
    }

    public Tag()
    {
    }

    public Guid Id { get; private init; }
    public Guid PostId { get; private init; }
    public string Title { get; private init; }
}
