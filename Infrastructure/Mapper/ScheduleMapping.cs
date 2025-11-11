using AutoMapper;
using Domain.Enum;
using Domain.Model;
using Infrastructure.ViewModel.Request;
using static Infrastructure.ViewModel.Response.AccountResponse;
using static Infrastructure.ViewModel.Response.CropResponse;
using static Infrastructure.ViewModel.Response.DailyLogResponse;
using static Infrastructure.ViewModel.Response.FarmActivityResponse;
using static Infrastructure.ViewModel.Response.FarmDetailResponse;
using static Infrastructure.ViewModel.Response.ScheduleResponse;

namespace Infrastructure.Mapper
{
    public class ScheduleMapping : Profile
    {
        public ScheduleMapping()
        {
            CreateMap<Schedule, ScheduleRequest>().ReverseMap()
                   .ForMember(dest => dest.CropId, opt => opt.MapFrom(src => src.CropId))
                   .ForMember(dest => dest.AssignedTo, opt => opt.MapFrom(src => src.StaffId))
                ;
            CreateMap<Schedule, ScheduleResponseView>().ReverseMap()
                .ForMember(dest => dest.Crop, opt => opt.MapFrom(src => src.cropView))
                .ForMember(dest => dest.FarmDetails, opt => opt.MapFrom(src => src.farmView))
                .ForPath(dest => dest.AssignedToNavigation.AccountProfile, opt => opt.MapFrom(src => src.Staff))

              .ForMember(dest => dest.FarmActivities,
                        opt => opt.MapFrom(src => src.farmActivityView == null ? null : src.farmActivityView))
            ;

            CreateMap<FarmActivity, FarmActivityView>().ReverseMap();
            CreateMap<Crop, CropView>().ReverseMap();
            CreateMap<Farm, FarmView>().ReverseMap();
            //  CreateMap<DailyLog, DailyLogView>().ReverseMap();

            CreateMap<Account, ViewAccount>().ReverseMap()
                .ForMember(dest => dest.AccountProfile, opt => opt.Condition(src => src.AccountProfile != null));

        }
    }
}
