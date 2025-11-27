using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Mapper
{
    public class CropRequirementMapping : Profile
    {
        public CropRequirementMapping()
        {
            // CreateMap<Source, Destination>();
            CreateMap<Domain.Model.CropRequirement, Infrastructure.ViewModel.Response.CropRequirementResponse.CropRequirementView>()
                .ForMember(dest => dest.PlantStage, opt => opt.MapFrom(src => src.PlantStage.HasValue ? src.PlantStage.Value.ToString() : null));

            CreateMap<Infrastructure.ViewModel.Request.CropRequirementRequest, Domain.Model.CropRequirement>();
        }
    }
}
