using System;
using System.Runtime.Serialization;

namespace Blog.PublicAPI.Domain.PostAggregate;

[Serializable]
public class PostMustBeUniqueException : Exception
{
    public PostMustBeUniqueException()
    {
    }

    public PostMustBeUniqueException(string message)
        : base(message)
    {
    }

    public PostMustBeUniqueException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    protected PostMustBeUniqueException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
