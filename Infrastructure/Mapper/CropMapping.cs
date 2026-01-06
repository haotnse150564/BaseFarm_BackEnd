using AutoMapper;
using Domain.Model;
using Infrastructure.ViewModel.Request;
using Infrastructure.ViewModel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.CropRequirementResponse;
using static Infrastructure.ViewModel.Response.CropResponse;

namespace Infrastructure.Mapper
{
    public class CropMapping : Profile
    {
        public CropMapping()
        {
            CreateMap<CropRequirement, CropRequirementView>()
                .ForMember(dest => dest.CropRequirementId, opt => opt.MapFrom(src => src.CropRequirementId))
                .ForMember(dest => dest.CropId, opt => opt.MapFrom(src => src.CropId))
                .ForMember(dest => dest.PlantStage,
                    opt => opt.MapFrom(src => src.PlantStage.HasValue ? src.PlantStage.Value.ToString() : null))
                .ForMember(dest => dest.EstimatedDate, opt => opt.MapFrom(src => src.EstimatedDate))
                .ForMember(dest => dest.SoilMoisture, opt => opt.MapFrom(src => src.SoilMoisture))
                .ForMember(dest => dest.Humidity, opt => opt.MapFrom(src => src.Humidity))
                .ForMember(dest => dest.Temperature, opt => opt.MapFrom(src => src.Temperature))
                .ForMember(dest => dest.Fertilizer, opt => opt.MapFrom(src => src.Fertilizer))
                .ForMember(dest => dest.LightRequirement, opt => opt.MapFrom(src => src.LightRequirement))
                .ForMember(dest => dest.WateringFrequency,
                    opt => opt.MapFrom(src => src.WateringFrequency.HasValue ? src.WateringFrequency.Value.ToString() : null))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes));

            CreateMap<Crop, CropView>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ReverseMap();


            CreateMap<Crop, CropRequest>().ReverseMap();

            
        }
    }
}
