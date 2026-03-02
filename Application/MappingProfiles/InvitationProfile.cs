using Application.Dtos.InvitationDtos;
using AutoMapper;
using Domain.Models;

namespace Application.MappingProfiles;

public class InvitationProfile : Profile
{
    public InvitationProfile()
    {
        CreateMap<Invitation, InvitationDto>()
            .ForMember(des => des.Status, options => options
            .MapFrom(src => src.IsActive || src.Status == InvitationStatusEnum.Accepted ? src.Status : InvitationStatusEnum.Expired));
    }
}
