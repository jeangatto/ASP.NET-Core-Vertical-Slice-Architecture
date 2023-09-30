using System;
using System.Collections.Generic;

namespace Blog.PublicAPI.Features.Posts;

public class PostResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public IEnumerable<string> Tags { get; set; }
}