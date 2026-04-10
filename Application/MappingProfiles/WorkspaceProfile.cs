using Application.Dtos.WorkspaceDtos;
using AutoMapper;
using Domain.Models;

namespace Application.MappingProfiles
{
    public class WorkspaceProfile : Profile
    {
        public WorkspaceProfile()
        {
            CreateMap<Workspace, WorkspaceSimpleDto>()
                .ForMember(dest => dest.OwnerName, opt =>
                opt.MapFrom(src =>
                src.User.Name))
                .ForMember(dest => dest.MembersCount, opt =>
                opt.MapFrom(src => src.WorkspaceMembers.Count));

            CreateMap<Workspace, WorkspaceDetailsDto>()
               .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.User.Name))
            .ForMember(dest => dest.Spaces, opt => opt.MapFrom(src => src.Spaces))
            .ForMember(dest => dest.Members, opt => opt.MapFrom(src => src.WorkspaceMembers))
            .ForMember(dest => dest.TotalSpaces, opt => opt.MapFrom(src => src.Spaces.Count))
            .ForMember(dest => dest.TotalMembers, opt => opt.MapFrom(src => src.WorkspaceMembers.Count))
            .ForMember(dest => dest.TotalTags, opt => opt.MapFrom(src => src.Tags.Count));
        }
    }
}
