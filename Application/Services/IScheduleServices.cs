using Infrastructure.ViewModel.Request;
using Infrastructure.ViewModel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IScheduleServices
    {
        Task<ScheduleResponse> GetAllScheduleAsync(int pageIndex, int pageSize);
        Task<ScheduleResponse> GetScheduleByIdAsync(long ScheduleId);
        Task<ScheduleResponse> GetScheduleByNameAsync(string ScheduleName, int pageIndex, int pageSize);
        Task<ScheduleResponse> CreateScheduleAsync(ScheduleRequest request);
        Task<ScheduleResponse> UpdateScheduleById(long ScheduleId, ScheduleRequest request);
        Task<ScheduleResponse> ChangeScheduleStatusById(long ScheduleId);
    }
}
