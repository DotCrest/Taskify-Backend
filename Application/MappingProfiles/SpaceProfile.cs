using Application.Dtos.SpaceDtos;
using AutoMapper;
using Domain.Models;
namespace Application.MappingProfiles;

public class SpaceProfile : Profile
{
    public SpaceProfile()
    {
        CreateMap<Space, SimpleSpaceDto>();
    }
}
