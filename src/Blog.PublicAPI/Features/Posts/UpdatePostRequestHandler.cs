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

public class UpdatePostRequestHandler : IRequestHandler<UpdatePostRequest, Result<PostResponse>>
{
    private readonly IMapper _mapper;
    private readonly IPostRepository _repository;
    private readonly IValidator<UpdatePostRequest> _validator;

    public UpdatePostRequestHandler(IMapper mapper, IPostRepository repository, IValidator<UpdatePostRequest> validator)
    {
        _mapper = mapper;
        _repository = repository;
        _validator = validator;
    }

    public async Task<Result<PostResponse>> Handle(UpdatePostRequest request, CancellationToken cancellationToken)
    {
        var result = await _validator.ValidateAsync(request, cancellationToken);
        if (!result.IsValid)
        {
            return Result.Invalid(result.AsErrors());
        }

        if (await _repository.ExistsAsync(request.Title, request.Id))
        {
            return Result.Invalid(new List<ValidationError>
            {
                new ValidationError
                {
                    ErrorMessage = "There is already a registered post with the given title"
                }
            });
        }

        var post = await _repository.GetByIdAsync(request.Id);
        if (post == null)
        {
            return Result.NotFound($"No posts found by id = {request.Id}");
        }

        post.Update(request.Title, request.Content, request.Tags);

        await _repository.UpdateAsync(post);

        return Result.Success(_mapper.Map<PostResponse>(post));
    }
}