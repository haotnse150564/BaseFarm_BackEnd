using AutoMapper;
using Domain.Model;
using Infrastructure.ViewModel.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.CropResponse;

namespace Infrastructure.Mapper
{
    public class CropMapping : Profile
    {
        public CropMapping() {
            CreateMap<Crop, CropView>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ReverseMap();
                
            CreateMap<Crop, CropRequest>().ReverseMap();
        }
    }
}
