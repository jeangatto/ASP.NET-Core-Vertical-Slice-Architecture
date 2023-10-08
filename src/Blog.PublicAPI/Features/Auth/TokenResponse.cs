using System;

namespace Blog.PublicAPI.Features.Auth;

public class TokenResponse
{
    public string AccessToken { get; set; }
    public DateTime Created { get; set; }
    public DateTime Expiration { get; set; }
    public string RefreshToken { get; set; }

    public int ExpiresIn => (int)Expiration.Subtract(Created).TotalSeconds;
}