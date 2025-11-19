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
            CreateMap<Crop, CropView>().ReverseMap();
            CreateMap<Farm, FarmView>().ReverseMap();
            CreateMap<FarmActivity, FarmActivityView>().ReverseMap();
            CreateMap<AccountProfile, ViewAccount>().ReverseMap();


            CreateMap<Schedule, ScheduleRequest>()
                   .ForMember(dest => dest.CropId, opt => opt.MapFrom(src => src.CropId))
                   .ForMember(dest => dest.StaffId, opt => opt.MapFrom(src => src.AssignedTo))
                ;
            CreateMap<Schedule, ScheduleResponseView>()
                .ForMember(dest => dest.cropView, opt => opt.MapFrom(src => src.Crop))
                .ForMember(dest => dest.farmView, opt => opt.MapFrom(src => src.FarmDetails))
                .ForMember(dest => dest.Staff, opt => opt.MapFrom(src => src.AssignedToNavigation.AccountProfile))
                .ForMember(dest => dest.farmActivityView, opt => opt.MapFrom(src => src.FarmActivities))
                .ForPath(destinationMember => destinationMember.StaffName, opt => opt.MapFrom(src => src.AssignedToNavigation.AccountProfile.Fullname))
                .ForPath(dest => dest.ManagerName, opt => opt.MapFrom(src => src.AssignedToNavigation.AccountProfile.Fullname));


            CreateMap<ScheduleResponseView, Schedule>()
                .ForMember(dest => dest.Crop, opt => opt.MapFrom(src => src.cropView))
                .ForMember(dest => dest.FarmDetails, opt => opt.MapFrom(src => src.farmView))
                .ForMember(dest => dest.FarmActivities, opt => opt.MapFrom(src => src.farmActivityView))
                 .ForPath(dest => dest.AssignedToNavigation.AccountProfile, opt => opt.MapFrom(src => src.Staff))
                .ForPath(dest => dest.AssignedToNavigation.AccountProfile.Fullname, opt => opt.MapFrom(src => src.ManagerName));

            //  CreateMap<DailyLog, DailyLogView>().ReverseMap();

            CreateMap<Account, ViewAccount>().ReverseMap()
                .ForMember(dest => dest.AccountProfile, opt => opt.Condition(src => src.AccountProfile != null));

        }
    }
}
