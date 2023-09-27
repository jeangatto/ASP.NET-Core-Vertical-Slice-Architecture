using System.Linq;
using AutoMapper;
using Blog.PublicAPI.Domain.PostAggregate;

namespace Blog.PublicAPI.Features.Posts;

public class PostProfile : Profile
{
    public PostProfile()
    {
        CreateMap<Post, PostResponse>(MemberList.Destination)
            .ForMember(dest => dest.Tags, cfg => cfg.MapFrom(src => src.Tags.Select(tag => $"#{tag.Title}").ToList()));
    }
}
