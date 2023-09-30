using AutoMapper;
using Blog.PublicAPI.Domain.UserAggregate;

namespace Blog.PublicAPI.Features.Users;

public class UserMapperProfile : Profile
{
    public UserMapperProfile()
    {
        CreateMap<User, UserResponse>(MemberList.Destination);
    }

    public override string ProfileName => nameof(UserMapperProfile);
}