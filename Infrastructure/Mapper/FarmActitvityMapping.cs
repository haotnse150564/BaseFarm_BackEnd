using AutoMapper;
using Domain.Model;
using Infrastructure.ViewModel.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.AccountResponse;
using static Infrastructure.ViewModel.Response.FarmActivityResponse;

namespace Infrastructure.Mapper
{
    public class FarmActitvityMapping : Profile
    {
        public FarmActitvityMapping()
        {
            CreateMap<FarmActivity, FarmActivityView>()
                .ForMember(dest => dest.FarmActivitiesId, opt => opt.MapFrom(src => src.FarmActivitiesId))
                .ForMember(dest => dest.ActivityType, opt => opt.MapFrom(src => src.ActivityType.ToString()))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.HasValue ? src.StartDate.Value.ToString("yyyy-MM-dd") : null))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.HasValue ? src.EndDate.Value.ToString("yyyy-MM-dd") : null))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.StaffDTO, opt => opt.MapFrom(src => src.AssignedToNavigation))

                .ReverseMap();
            CreateMap<FarmActivity, FarmActivityRequest>()
                .ForMember(dest => dest.StaffId, opt => opt.MapFrom(src => src.AssignedTo))
                .ForMember(dest => dest.ScheduleId, opt => opt.MapFrom(src => src.scheduleId))

                .ReverseMap();

            CreateMap<Account, AvailableStaffDTO>()
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.AccountProfile.Fullname))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.AccountProfile.Phone))
                            .ReverseMap();
        }
    }
}
