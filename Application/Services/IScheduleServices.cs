using Infrastructure.ViewModel.Request;
using Infrastructure.ViewModel.Response;
using static Infrastructure.ViewModel.Response.ScheduleResponse;

namespace Application.Services
{
    public interface IScheduleServices
    {
        Task<ResponseDTO> GetAllScheduleAsync(int pageIndex, int pageSize);
        Task<ScheduleResponse> GetScheduleByIdAsync(long ScheduleId);
        Task<ScheduleResponse> CreateScheduleAsync(ScheduleRequest request);
        Task<ScheduleResponse> UpdateScheduleById(long ScheduleId, ScheduleRequest request);
        Task<ScheduleResponse> ChangeScheduleStatusById(long ScheduleId);
    }
}
