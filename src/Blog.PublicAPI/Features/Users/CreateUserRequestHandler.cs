using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using AutoMapper;
using BCrypt.Net;
using Blog.PublicAPI.Domain.UserAggregate;
using FluentValidation;
using MediatR;

namespace Blog.PublicAPI.Features.Users;

public class CreateUserRequestHandler : IRequestHandler<CreateUserRequest, Result<UserResponse>>
{
    private readonly IMapper _mapper;
    private readonly IUserRepository _repository;
    private readonly IValidator<CreateUserRequest> _validator;

    public CreateUserRequestHandler(IMapper mapper, IUserRepository repository, IValidator<CreateUserRequest> validator)
    {
        _mapper = mapper;
        _repository = repository;
        _validator = validator;
    }

    public async Task<Result<UserResponse>> Handle(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _validator.ValidateAsync(request, cancellationToken);
        if (!result.IsValid)
        {
            return Result.Invalid(result.AsErrors());
        }

        if (await _repository.ExistsAsync(request.Email))
        {
            return Result.Invalid(new List<ValidationError>
            {
                new ValidationError
                {
                    ErrorMessage = "The email address provided is already in use"
                }
            });
        }

        var hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(request.Password, HashType.SHA512);

        var user = User.Create(request.Name, request.Email, hashedPassword);

        await _repository.AddAsync(user);

        return Result.Success(_mapper.Map<UserResponse>(user));
    }
}