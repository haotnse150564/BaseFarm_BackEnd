using AutoMapper;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.FarmDetailResponse;

namespace Infrastructure.Mapper
{
   public class FarmMapping : Profile
    {
        public FarmMapping()
        {
            CreateMap<Farm, FarmDetailView>().ReverseMap();
        }
    }
  
}
