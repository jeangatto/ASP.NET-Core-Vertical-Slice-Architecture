using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.PublicAPI.Domain.PostAggregate;

public class Post : IEntity<Guid>, IAggregateRoot
{
    private readonly List<Tag> _tags = new();

    private Post(string title, string content, string[] tags)
    {
        Id = Guid.NewGuid();
        Title = title;
        Content = content;
        CreatedAt = DateTime.UtcNow;

        AddTags(tags);
    }

    public Post()
    {
    }

    public Guid Id { get; }
    public string Title { get; private set; }
    public string Content { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();

    public static async Task<Post> CreateAsync(string title, string content, string[] tags, IPostUniquenessChecker checker)
    {
        ArgumentException.ThrowIfNullOrEmpty(title, nameof(title));
        ArgumentException.ThrowIfNullOrEmpty(content, nameof(title));
        ArgumentNullException.ThrowIfNull(tags, nameof(tags));

        await checker.ValidateTitleIsUniqueAsync(title);

        var post = new Post(title, content, tags);

        return post;
    }

    public async Task UpdateAsync(string title, string content, string[] tags, IPostUniquenessChecker checker)
    {
        ArgumentException.ThrowIfNullOrEmpty(title, nameof(title));
        ArgumentException.ThrowIfNullOrEmpty(content, nameof(title));
        ArgumentNullException.ThrowIfNull(tags, nameof(tags));

        await checker.ValidateTitleIsUniqueAsync(title, this);

        Title = title;
        Content = content;
        UpdatedAt = DateTime.UtcNow;

        AddTags(tags);
    }

    private void AddTags(string[] tags)
    {
        _tags.Clear();

        var newTags = tags
            .Distinct()
            .OrderBy(tag => tag)
            .Select(tag => new Tag(Id, tag))
            .ToList();

        _tags.AddRange(newTags);
    }

    public override string ToString() => $"Id = {Id}, Title = {Title}, Tags = {string.Join(",", Tags)}";
}
