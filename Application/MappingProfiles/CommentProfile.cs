using Application.Dtos.CommentDto;
using AutoMapper;
using Domain.Models;

namespace Application.MappingProfiles;

public class CommentProfile : Profile
{
    public CommentProfile()
    {
        CreateMap<AddCommentDto, Comment>()
           .ForMember(des => des.Id, opt => opt.MapFrom(_ => Guid.NewGuid().ToString()))
           .ForMember(des => des.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
    }
}
