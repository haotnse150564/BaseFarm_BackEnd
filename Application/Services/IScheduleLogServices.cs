using Application.Commons;
using Infrastructure.ViewModel.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.ViewModel.Response.OrderResponse;

namespace Application.Services
{
    public interface IScheduleLogServices
    {
        Task<ResponseDTO> GetLogsByScheduleIdAsync(long scheduleId, int pageIndex = 0, int pageSize = 10);
        Task<ResponseDTO> CreateManualLogAsync(CreateScheduleLogRequest request);
        Task<ResponseDTO> UpdateManualLogAsync(UpdateScheduleLogRequest request);
    }
}
