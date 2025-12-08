using AutoMapper;
using Domain.Enum;
using Domain.Model;
using Infrastructure.ViewModel.Request;
using Infrastructure.ViewModel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.CropRequirementResponse;

namespace Infrastructure.Mapper
{
    public class CropRequirementMapping : Profile
    {
        public CropRequirementMapping()
        {
            // CreateMap<Source, Destination>();
            CreateMap<CropRequirement, CropRequirementView>().ReverseMap()
                .ForMember(dest => dest.PlantStage,
           opt => opt.MapFrom(src => src.PlantStage.ToString()));


            CreateMap<CropRequirementRequest, CropRequirement>().ReverseMap()
                .ForMember(des => des.WateringFrequency, opt => opt.MapFrom(src => src.WateringFrequency));

            CreateMap<CropRequirement, CropRequirementDto>()
                .ForMember(dest => dest.PlantStage,
                    opt => opt.MapFrom(src => src.PlantStage.GetValueOrDefault(PlantStage.Germination))) 
                .ForMember(dest => dest.EstimatedDate,
                    opt => opt.MapFrom(src => src.EstimatedDate.GetValueOrDefault(0)))
                .ForMember(dest => dest.WateringFrequency,
                    opt => opt.MapFrom(src => src.WateringFrequency));
        }
    }
}
