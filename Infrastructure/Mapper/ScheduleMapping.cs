using AutoMapper;
using Domain.Enum;
using Domain.Model;
using Infrastructure.ViewModel.Request;
using Infrastructure.ViewModel.Response;
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

            // CreateMap<DailyLog, DailyLogView>().ReverseMap();

            CreateMap<Account, ViewAccount>().ReverseMap()
                .ForMember(dest => dest.AccountProfile, opt => opt.Condition(src => src.AccountProfile != null));

            CreateMap<(Schedule schedule, DateOnly today, int daysSinceStart, CropRequirement? current, CropRequirement? next, int? daysToNext), UpdateTodayResponse>()
                .ForMember(dest => dest.Today, opt => opt.MapFrom(src => src.today))
                .ForMember(dest => dest.CurrentStage, opt => opt.MapFrom(src => src.schedule.currentPlantStage))
                .ForMember(dest => dest.DaysSinceStart, opt => opt.MapFrom(src => src.daysSinceStart))
                .ForMember(dest => dest.DaysToNextStage, opt => opt.MapFrom(src => src.daysToNext))
                .ForMember(dest => dest.CurrentStageRequirements, opt => opt.MapFrom(src => src.current))
                .ForMember(dest => dest.NextStageRequirements, opt => opt.MapFrom(src => src.next));

            // Chuyển từ CropRequirement (entity) sang CropRequirementDto (có PlantStage là enum, WateringFrequency là int?)
            CreateMap<CropRequirement, CropRequirementResponse.CropRequirementDto>()
                .ForMember(dest => dest.PlantStage,
                    opt => opt.MapFrom(src => src.PlantStage.GetValueOrDefault(PlantStage.Germination)))
                .ForMember(dest => dest.EstimatedDate,
                    opt => opt.MapFrom(src => src.EstimatedDate.GetValueOrDefault(0)))
                .ForMember(dest => dest.WateringFrequency,
                    opt => opt.MapFrom(src => src.WateringFrequency)); 
        }
    }
}
