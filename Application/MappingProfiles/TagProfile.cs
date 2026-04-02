using Application.Dtos.TagDtos;
using AutoMapper;
using Domain.Models;

namespace Application.MappingProfiles;

public class TagProfile : Profile
{
    public TagProfile()
    {
        CreateMap<Tag, TagToReturnDto>();
        CreateMap<TagDto, Tag>()
            .ForMember(des => des.Color, opt => opt.MapFrom(src => src.Color ?? "#808080"));
    }
}
