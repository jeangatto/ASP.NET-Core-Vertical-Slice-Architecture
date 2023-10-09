namespace Blog.PublicAPI.Features.Authentication;

public record TokenResponse(string AccessToken, int ExpiresIn);