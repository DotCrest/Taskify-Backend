using Application.Dtos.WorkspaceMemberDtos;
using AutoMapper;
using Domain.Models;

namespace Application.MappingProfiles;

public class WorkspaceMemberProfile : Profile
{
    public WorkspaceMemberProfile()
    {
        CreateMap<WorkspaceMember, WorkspaceMemberDto>()
            .ForMember(dest => dest.UserId, opt =>
            opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.UserName, opt =>
            opt.MapFrom(src => src.User.UserName))
            .ForMember(dest => dest.Email, opt =>
            opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.Role, opt =>
            opt.MapFrom(src => src.Role.ToString()));
    }
}
