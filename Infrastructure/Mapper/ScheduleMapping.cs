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
            CreateMap<Schedule, ScheduleRequest>().ReverseMap();

            CreateMap<ViewSchedule, Schedule>().ReverseMap()
                   .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                   .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.ToString()))
                   .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.ToString()))
                   .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToString()))
                   .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.ToString()))
                   .ForMember(dest => dest.AssignedTo, opt => opt.MapFrom(src => src.AssignedTo))
                   .ForMember(dest => dest.FarmActivityId, opt => opt.MapFrom(src => src.FarmActivityId))
                   .ForMember(dest => dest.FarmId, opt => opt.MapFrom(src => src.FarmDetailsId))

                   .ForMember(dest => dest.accountView, opt => opt.MapFrom(src => src.AssignedToNavigation))
                   .ForMember(dest => dest.farmActivityView, opt => opt.MapFrom(src => src.FarmActivity))
                   .ForMember(dest => dest.cropView, opt => opt.MapFrom(src => src.Crop))
                   .ForMember(dest => dest.dailyLog, opt => opt.MapFrom(src => src.DailyLogs));
            ;

            CreateMap<FarmActivity, FarmActivityView>().ReverseMap();
            CreateMap<Crop, CropView>().ReverseMap();
            CreateMap<Farm, FarmView>().ReverseMap();
            CreateMap<DailyLog, DailyLogView>().ReverseMap();

            CreateMap<Account, ViewAccount>().ReverseMap()
                .ForMember(dest => dest.AccountProfile, opt => opt.Condition(src => src.AccountProfile != null));

        }
    }
}
