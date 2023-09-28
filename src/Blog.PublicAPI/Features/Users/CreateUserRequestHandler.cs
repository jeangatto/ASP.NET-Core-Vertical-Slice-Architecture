using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using MediatR;

namespace Blog.PublicAPI.Features.Users;

public class CreateUserRequestHandler : IRequestHandler<CreateUserRequest, Result<UserResponse>>
{
    public Task<Result<UserResponse>> Handle(CreateUserRequest request, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}