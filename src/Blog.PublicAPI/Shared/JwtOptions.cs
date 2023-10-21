namespace Blog.PublicAPI.Shared;

public class JwtOptions
{
    public string SigningKey { get; private init; }
    public string Issuer { get; private init; }
    public string Audience { get; private init; }
    public int ExpirationSeconds { get; private init; }
}