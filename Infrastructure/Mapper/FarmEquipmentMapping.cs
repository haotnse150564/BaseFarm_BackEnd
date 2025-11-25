using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Mapper
{
    public class FarmEquipmentMapping : Profile
    {
        public FarmEquipmentMapping()
        {
            // Create your mappings here
            CreateMap<Domain.Model.FarmEquipment, Infrastructure.ViewModel.Response.FarmEquipmentResponse.FarmEquipmentView>()
                .ForMember(dest => dest.FarmEquipmentId, opt => opt.MapFrom(src => src.FarmEquipmentId))
                .ForMember(dest => dest.DeviceName, opt => opt.MapFrom(src => src.Device.DeviceName))
                .ForMember(dest => dest.FarmName, opt => opt.MapFrom(src => src.Farm.FarmName))
                .ForMember(dest => dest.AssignDate, opt => opt.MapFrom(src => src.AssignDate))
                .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ReverseMap();
            CreateMap<Infrastructure.ViewModel.Request.FarmEquipmentRequest, Domain.Model.FarmEquipment>()
                .ForMember(dest => dest.DeviceId, opt => opt.MapFrom(src => src.deviceId))
                .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note))
                .ReverseMap();
        }
    }
}
