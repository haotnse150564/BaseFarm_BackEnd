﻿using AutoMapper;
using Domain.Model;
using static Infrastructure.ViewModel.Request.AccountProfileRequest;
using static Infrastructure.ViewModel.Response.AccountProfileResponse;

namespace Infrastructure.Mapper
{
    public class AccountProfileMapping : Profile
    {
        public AccountProfileMapping()
        {
            CreateMap<AccountProfile, ProfileResponseDTO>()
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Fullname, opt => opt.MapFrom(src => src.Fullname))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ReverseMap();

            CreateMap<ProfileRequestDTO, AccountProfile>()
                .ReverseMap();
        }
    }
}
