using System;

namespace Blog.PublicAPI.Domain.PostAggregate;

public class Tag : IEntity<Guid>
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

    public Guid Id { get; }
    public Guid PostId { get; private init; }
    public string Title { get; private init; }

    public override string ToString() => Title;
}
