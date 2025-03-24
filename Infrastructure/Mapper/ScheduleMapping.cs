using AutoMapper;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.ScheduleResponse;

namespace Infrastructure.Mapper
{
    public class ScheduleMapping :Profile
    {
        public ScheduleMapping()
        {
            CreateMap<Schedule, ViewSchedule>().ReverseMap();
        }
    }
}
