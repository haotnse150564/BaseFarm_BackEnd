using AutoMapper;
using Domain.Model;
using Infrastructure.ViewModel.Request;
using Infrastructure.ViewModel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.AccountResponse;

namespace Infrastructure.Mapper
{
    public class AddressMapping : Profile
    {
        public AddressMapping()
        {
            CreateMap<Address, AddressRequest>().ReverseMap();
            CreateMap<Address, AddressReponse>()
                .ForMember(dest => dest.RecipientName, opt => opt.MapFrom(src => src.Account.AccountProfile.Fullname))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Account.AccountProfile.Phone))
                .ReverseMap();


            CreateMap<Account, ViewAccount>()
                .ForMember(dest => dest.AccountProfile, opt => opt.Condition(src => src.AccountProfile != null))
                .ReverseMap();
        }
    }
}
