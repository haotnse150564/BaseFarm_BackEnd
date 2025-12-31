using Application.Repositories;
using Domain.Model;
using Infrastructure.ViewModel.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.ViewModel.Response.OrderResponse;

namespace Infrastructure.Repositories
{
    public interface IScheduleLogRepository : IGenericRepository<ScheduleLog>
    {
        Task<List<ScheduleLog>> GetAllByScheduleIdAsync(long scheduleId);
       
    }
}
