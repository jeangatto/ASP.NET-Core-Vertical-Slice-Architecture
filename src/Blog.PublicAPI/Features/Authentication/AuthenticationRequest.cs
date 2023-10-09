using Ardalis.Result;
using MediatR;

namespace Blog.PublicAPI.Features.Authentication;

public record AuthenticationRequest(string Email, string Password) : IRequest<Result<TokenResponse>>;