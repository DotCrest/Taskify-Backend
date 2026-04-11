using Application.Dtos.CommentDto;
using AutoMapper;
using Domain.Models;

namespace Application.MappingProfiles;

public class CommentProfile : Profile
{
    public CommentProfile()
    {
        CreateMap<AddCommentDto, Comment>()
            .ForMember(des => des.Content, opt => opt.MapFrom(src => src.UserComment))

            .ForMember(des => des.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<Comment, CommentDto>();

        CreateMap<UpdateCommentDto, Comment>()
            .ForMember(des => des.Content, opt =>
            {
                opt.Condition(src => !string.IsNullOrEmpty(src.UserComment));
                opt.MapFrom(src => src.UserComment);
            })
            .ForMember(des => des.Id, opt => opt.Ignore())
            .ForMember(des => des.UserId, opt => opt.Ignore())
            .ForMember(des => des.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
    }
}
