using AutoMapper;
using Domain.Enum;
using Domain.Model;
using Infrastructure.ViewModel.Request;
using System.ComponentModel;
using VNPAY.NET.Utilities;
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
                   .ReverseMap();

            CreateMap<Schedule, ScheduleResponseView>()
                .ForMember(dest => dest.cropView, opt => opt.MapFrom(src => src.Crop))
                .ForMember(dest => dest.farmView, opt => opt.MapFrom(src => src.FarmDetails))
                .ForMember(dest => dest.Staff, opt => opt.MapFrom(src => src.AssignedToNavigation.AccountProfile))
                .ForMember(dest => dest.farmActivityView, opt => opt.MapFrom(src => src.FarmActivities))
                .ForPath(destinationMember => destinationMember.StaffName, opt => opt.MapFrom(src => src.AssignedToNavigation.AccountProfile.Fullname))
                .ForPath(dest => dest.CropRequirement, opt => opt.MapFrom(src => src.Crop.CropRequirement))
                .ForPath(dest => dest.CurrentPlantStage, opt => opt.MapFrom(src => src.currentPlantStage))

                .ForPath(dest => dest.ManagerName, opt => opt.MapFrom(src => src.AssignedToNavigation.AccountProfile.Fullname))
                .ReverseMap();

            //  CreateMap<DailyLog, DailyLogView>().ReverseMap();

            CreateMap<Account, ViewAccount>().ReverseMap()
                .ForMember(dest => dest.AccountProfile, opt => opt.Condition(src => src.AccountProfile != null));
    }
    }
}
