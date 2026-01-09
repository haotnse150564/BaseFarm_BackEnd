using AutoMapper;
using Domain.Model;
using Infrastructure.ViewModel.Request;
using Infrastructure.ViewModel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.IOTLogResponse;
using static Infrastructure.ViewModel.Response.IOTResponse;

namespace Infrastructure.Mapper
{
    public class IOTMapping : Profile
    {
        public IOTMapping()
        {
            //CreateMap<Source, Destination>().ReverseMap();
            CreateMap<Device, IOTRequest>()
                .ForMember(dest => dest.PinCode, opt => opt.MapFrom(src => src.Pin))

                .ReverseMap();
            CreateMap<Device, IOTView>()
                .ForMember(dest => dest.PinCode, opt => opt.MapFrom(src => src.Pin))
                .ForPath(dest => dest.FarmName, opt => opt.MapFrom(src => src.FarmEquipments.FirstOrDefault().Farm.FarmName))
                .ReverseMap();
            CreateMap<Device, IOTResponse>().ReverseMap();
            CreateMap<IOTLog, IOTLogView>().ReverseMap();
        }
    }

}
