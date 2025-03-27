using AutoMapper;
using Domain.Enum;
using Domain.Model;
using Infrastructure.ViewModel.Request;
using static Infrastructure.ViewModel.Response.AccountResponse;
using static Infrastructure.ViewModel.Response.ScheduleResponse;

namespace Infrastructure.Mapper
{
    public class ScheduleMapping : Profile
    {
        public ScheduleMapping()
        {
            CreateMap<Schedule, ViewSchedule>().ReverseMap()
                   .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (Status)Enum.Parse(typeof(Status), src.Status, true)))
                   .ForMember(dest => dest.AssignedTo, opt => opt.MapFrom(src => src.AssignedTo));
            ;
            CreateMap<Schedule, ScheduleRequest>().ReverseMap();
        }
    }
}
