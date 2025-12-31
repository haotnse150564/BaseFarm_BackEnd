using AutoMapper;
using Domain.Model;
using Infrastructure.ViewModel.Request;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Mapper
{
    public class ScheduleLogMapping : Profile
    {
        public ScheduleLogMapping()
        {
            CreateMap<ScheduleLog, ScheduleLogDto>().ReverseMap();
        }
    }
}
