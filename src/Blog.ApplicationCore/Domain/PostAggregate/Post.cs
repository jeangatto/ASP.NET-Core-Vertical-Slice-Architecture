using System;
using System.Collections.Generic;
using System.Linq;

namespace Blog.ApplicationCore.Domain.PostAggregate;

public sealed class Post : IAggregateRoot
{
    private readonly List<Tag> _tags = new();

    private Post() { }

    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Content { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();

    public void Update(string title, string content, string[] tags)
    {
        Title = title;
        Content = content;
        UpdatedAt = DateTime.UtcNow;

        _tags.Clear();

        AddTags(tags);
    }

    public static Post Create(string title, string content, string[] tags)
    {
        var post = new Post
        {
            Id = Guid.NewGuid(),
            Title = title,
            Content = content,
            CreatedAt = DateTime.UtcNow
        };

        post.AddTags(tags);

        return post;
    }

    private void AddTags(string[] tags)
    {
        var newTags = tags
            .Distinct()
            .OrderBy(tag => tag)
            .Select(tag => Tag.Create(Id, tag))
            .ToList();

        _tags.AddRange(newTags);
    }
}