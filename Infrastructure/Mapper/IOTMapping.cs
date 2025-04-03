using AutoMapper;
using Domain.Model;
using Infrastructure.ViewModel.Request;
using Infrastructure.ViewModel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.IOTResponse;

namespace Infrastructure.Mapper
{
    public class IOTMapping : Profile
    {
        public IOTMapping()
        {
            //CreateMap<Source, Destination>().ReverseMap();
            CreateMap<IoTdevice, IOTRequest>().ReverseMap();
            CreateMap<IoTdevice, IOTView>().ReverseMap();
            CreateMap<IoTdevice, IOTResponse>().ReverseMap();
        }
    }

}
