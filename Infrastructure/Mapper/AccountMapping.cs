using AutoMapper;
using Domain.Model;
using Infrastructure.ViewModel.Response;
using static Infrastructure.ViewModel.Request.AccountRequest;
using static Infrastructure.ViewModel.Response.AccountResponse;

namespace Infrastructure.Mapper
{
    public class AccountMapping : Profile
    {
        public AccountMapping()
        {
            CreateMap<AccountProfile, AccountProfileResponse.ProfileResponseDTO>()
                   .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.Value))
                   .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToString()))
                .ReverseMap();
            CreateMap<Account, ViewAccount>().ReverseMap()
                .ForMember(dest => dest.AccountProfile, opt => opt.Condition(src => src.AccountProfile != null))
                .ForPath(dest => dest.AccountProfile.Fullname, opt => opt.MapFrom(src => src.Fullname))
                .ForPath(dest => dest.AccountProfile.Phone, opt => opt.MapFrom(src => src.Phone));
        }
    }
}
