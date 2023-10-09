using System;

namespace Blog.PublicAPI.Domain.PostAggregate;

public class Tag : IEntity<Guid>
{
    public Tag()
    {
    }

    private Tag(Guid postId, string title)
    {
        Id = Guid.NewGuid();
        PostId = postId;
        Title = title;
    }

    public Guid Id { get; }
    public Guid PostId { get; private init; }
    public string Title { get; private init; }

    public static Tag Create(Guid postId, string title)
    {
        ArgumentNullException.ThrowIfNull(postId, nameof(postId));
        ArgumentException.ThrowIfNullOrEmpty(title, nameof(title));

        return new Tag(postId, title.Replace(" ", "_").ToLowerInvariant().Trim());
    }

    public override string ToString() => Title;
}