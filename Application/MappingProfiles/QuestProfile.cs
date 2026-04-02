using Application.Dtos.QuestDtos;
using AutoMapper;
using Domain.Models;

namespace Application.MappingProfiles
{
    public class QuestProfile : Profile
    {
        public QuestProfile()
        {
            CreateMap<Quest, QuestToReturnDto>()
                .ForMember(dest => dest.Assignees,
                opt => opt.MapFrom(src => src.Assignees))
                .ForMember(dest => dest.AuthorName,
                opt => opt.MapFrom(src => src.Author != null ? src.Author.Name : string.Empty))
                .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
                .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Priority,
                opt => opt.MapFrom(src => src.Priority.ToString()));

        }
    }
}
