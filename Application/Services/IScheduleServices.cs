using Infrastructure.ViewModel.Request;
using Infrastructure.ViewModel.Response;
using System.Runtime.InteropServices;
using static Infrastructure.ViewModel.Response.ScheduleResponse;
using ResponseDTO = Infrastructure.ViewModel.Response.ScheduleResponse.ResponseDTO;

namespace Application.Services
{
    public interface IScheduleServices
    {

        Task<ResponseDTO> CreateSchedulesAsync(ScheduleRequest request);
        Task<ResponseDTO> UpdateSchedulesAsync(long ScheduleId, ScheduleRequest request);
        Task<ResponseDTO> GetAllSchedulesAsync(int pageIndex, int pageSize);
        Task<ResponseDTO> ScheduleByIdAsync(long ScheduleId);
        Task<ResponseDTO> ChangeScheduleStatusById(long ScheduleId);
        Task<ResponseDTO> AssignTask(long scheduleID, long staffId);
        Task<ResponseDTO> UpdateActivities(long ScheduleId, long[] activityId);
        Task<ResponseDTO> ScheduleStaffView(int month);

        Task<UpdateTodayResponse> UpdateTodayAsync(long scheduleId, UpdateTodayRequest request);
        //Task<ResponseDTO> AddFarmActivityToSchedule(long scheduleId, long farmActivities);
    }
}
