using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using AutoMapper;
using Blog.PublicAPI.Domain.PostAggregate;
using FluentValidation;
using MediatR;

namespace Blog.PublicAPI.Features.Posts;

public class CreatePostRequestHandler : IRequestHandler<CreatePostRequest, Result<PostResponse>>
{
    private readonly IMapper _mapper;
    private readonly IPostRepository _repository;
    private readonly IValidator<CreatePostRequest> _validator;

    public CreatePostRequestHandler(IMapper mapper, IPostRepository repository, IValidator<CreatePostRequest> validator)
    {
        _mapper = mapper;
        _repository = repository;
        _validator = validator;
    }

    public async Task<Result<PostResponse>> Handle(CreatePostRequest request, CancellationToken cancellationToken)
    {
        var result = await _validator.ValidateAsync(request, cancellationToken);
        if (!result.IsValid)
        {
            return Result.Invalid(result.AsErrors());
        }

        if (await _repository.ExistsAsync(request.Title))
        {
            return Result.Invalid(new List<ValidationError>
            {
                new ValidationError
                {
                    ErrorMessage = "There is already a registered post with the given title"
                }
            });
        }

        var postDomain = Post.Create(request.Title, request.Content, request.Tags);

        await _repository.AddAsync(postDomain);

        var response = _mapper.Map<PostResponse>(postDomain);

        return Result.Success(response);
    }
}