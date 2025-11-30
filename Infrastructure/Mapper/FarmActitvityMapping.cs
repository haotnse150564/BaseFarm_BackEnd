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
                .ForMember(dest => dest.plantStage, opt => opt.MapFrom(src => src.PlantStage))
                .ReverseMap();
            CreateMap<FarmActivity,FarmActivityRequest>().ReverseMap();
        }
    }
}
