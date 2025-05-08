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
            CreateMap<Crop, CropView>().ReverseMap();
            CreateMap<Crop, CropRequest>().ReverseMap();
        }
    }
}
