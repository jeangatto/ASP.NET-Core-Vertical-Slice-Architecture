using Ardalis.Result;
using MediatR;

namespace Blog.PublicAPI.Features.Auth;

public record LoginRequest(string Email, string Password) : IRequest<Result<TokenResponse>>;