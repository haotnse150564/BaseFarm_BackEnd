using AutoMapper;
using Domain.Model;
using Infrastructure.ViewModel.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.FarmActivityResponse;

namespace Infrastructure.Mapper
{
    public class FarmActitvityMapping : Profile
    {
        public FarmActitvityMapping() {
            CreateMap<FarmActivity, FarmActivityView>()
                .ForMember(dest => dest.FarmActivitiesId, opt => opt.MapFrom(src => src.FarmActivitiesId))
                .ForMember(dest => dest.ActivityType, opt => opt.MapFrom(src => src.ActivityType.ToString()))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                //                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.sta))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ReverseMap();
            CreateMap<FarmActivity,FarmActivityRequest>()
                .ForMember(dest => dest.StaffId, opt => opt.MapFrom(src => src.AssignedTo))
                .ForMember(dest => dest.ScheduleId, opt => opt.MapFrom(src => src.scheduleId))

                .ReverseMap();
        }
    }
}
