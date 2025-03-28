using Infrastructure.ViewModel.Request;
using Infrastructure.ViewModel.Response;
using static Infrastructure.ViewModel.Response.ScheduleResponse;

namespace Application.Services
{
    public interface IScheduleServices
    {
        Task<ResponseDTO> GetAllScheduleAsync(int pageIndex, int pageSize);
        Task<ResponseDTO> GetScheduleByIdAsync(long ScheduleId);
        Task<ResponseDTO> CreateScheduleAsync(ScheduleRequest request);
        Task<ResponseDTO> AssignStaff(long scheduleID, long staffId);
        Task<ResponseDTO> UpdateScheduleById(long ScheduleId, ScheduleRequest request);
        Task<ResponseDTO> ChangeScheduleStatusById(long ScheduleId, string status);
    }
}
