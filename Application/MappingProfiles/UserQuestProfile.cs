using Application.Dtos.UserQuestDtos;
using AutoMapper;
using Domain.Models;

namespace Application.MappingProfiles
{
    public class UserQuestProfile : Profile
    {
        public UserQuestProfile()
        {
            CreateMap<UserQuest, UserQuestDto>()
                .ForMember(dest => dest.UserName,
                opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.Avatar,
                opt => opt.MapFrom(src => src.User!.PhotoUrl))
                .ForMember(dest => dest.UserId,
                opt => opt.MapFrom(src => src.UserId));

            CreateMap<UserToQuestDto, UserQuest>()
                .ForMember(dest => dest.UserId,
                opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.QuestId,
                opt => opt.MapFrom(src => src.QuestId));
        }
    }
}
