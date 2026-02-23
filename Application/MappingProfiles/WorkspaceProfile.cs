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


        }
    }
}
