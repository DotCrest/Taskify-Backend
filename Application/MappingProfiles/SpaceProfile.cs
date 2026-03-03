using Application.Dtos.SpaceDtos;
using AutoMapper;
using Domain.Models;
namespace Application.MappingProfiles;

public class SpaceProfile : Profile
{
    public SpaceProfile()
    {
        CreateMap<Space, SimpleSpaceDto>();
        CreateMap<Space, SpaceDto>();
        CreateMap<PatchSpaceDto, Space>()
            .ForAllMembers(opt => opt.Condition((src, des, srcMember) => srcMember is not null));
    }
}
