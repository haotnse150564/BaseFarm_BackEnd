using Infrastructure.ViewModel.Request;
using Infrastructure.ViewModel.Response;
using ResponseDTO = Infrastructure.ViewModel.Response.ScheduleResponse.ResponseDTO;

namespace Application.Services
{
    public interface IScheduleServices
    {
    #region
        //Task<ResponseDTO> GetAllScheduleAsync(int pageIndex, int pageSize);
        //Task<ResponseDTO> GetScheduleByIdAsync(long ScheduleId);
        //Task<ResponseDTO> CreateScheduleAsync(ScheduleRequest request);
        //Task<ResponseDTO> AssignStaff(long scheduleID, long staffId);
        //Task<ResponseDTO> UpdateScheduleById(long ScheduleId, ScheduleRequest request);
        //Task<ResponseDTO> ChangeScheduleStatusById(long ScheduleId, string status);
        //Task<ResponseDTO> GetScheduleByCurrentStaffAsync(int month);
    #endregion
        Task<ResponseDTO> CreateSchedulesAsync(ScheduleRequest request);
        Task<ResponseDTO> UpdateSchedulesAsync(long ScheduleId, ScheduleRequest request);
        Task<ResponseDTO> GetAllSchedulesAsync(int pageIndex, int pageSize);
        Task<ResponseDTO> ScheduleByIdAsync(long ScheduleId);
        Task<ResponseDTO> ChangeScheduleStatusById(long ScheduleId, string status);
        Task<ResponseDTO> AssignTask(long scheduleID, long staffId);
        Task<ResponseDTO> UpdateActivities(long ScheduleId, long activityId);
        Task<ResponseDTO> ScheduleStaffView(int month);
    }
}
