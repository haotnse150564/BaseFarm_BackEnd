using AutoMapper;
using Domain.Model;
using Infrastructure.ViewModel.Request;
using Infrastructure.ViewModel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Mapper
{
    public class AddressMapping : Profile
    {
        public AddressMapping()
        {
            CreateMap<Address, AddressReponse>().ReverseMap();
            CreateMap<Address, AddressRequest>().ReverseMap();


        }
    }
}
