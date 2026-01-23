using Domain.Enum;
using Infrastructure.ViewModel.Request;
using Infrastructure.ViewModel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.FarmActivityResponse;
using static Infrastructure.ViewModel.Response.StaffActivityResponse;

namespace Application.Services
{
    public interface IFarmActivityServices
    {
        Task<ResponseDTO> GetFarmActivitiesAsync(int pageIndex, int pageSize, ActivityType? type, FarmActivityStatus? status, int? month);
        Task<ResponseDTO> GetFarmActivitiesByStaffAsync(int pageIndex, int pageSize, ActivityType? type, FarmActivityStatus? status, int? month);
        Task<ResponseDTO> GetFarmActivitiesActiveAsync(int pageIndex, int pageSize);
        Task<ResponseDTO> CreateFarmActivityAsync(FarmActivityRequest farmActivityRequest, ActivityType activityType);
        Task<ResponseDTO> UpdateFarmActivityAsync(long farmActivityId, FarmActivityRequest farmActivityRequest, ActivityType activityType, FarmActivityStatus farmActivityStatus);
        Task<ResponseDTO> GetFarmActivityByIdAsync(long farmActivityId);
        Task<ResponseDTO> ChangeFarmActivityStatusAsync(long farmActivityId);
        Task<ResponseDTO> CompleteFarmActivity(long id, string? location);

        Task<Response_DTO> AddStafftoFarmActivity(long farmActivityId, long staffId);
        Task<Response_DTO> UpdateStafftoFarmActivity(long Staf_farmActivityId);
        Task<Response_DTO> GetAllFarmTask();
        Task<Response_DTO> GetFarmTaskById(long taskId);
        Task<Response_DTO> GetStaffByFarmActivityId(long farmActivityId);
        Task<ResponseDTO> ReportMyPartCompletedAsync(long farmActivityId, string? notes = null);
        Task<ResponseDTO> GetFarmActivityByScheduleIdAsync(long farmActivityId);
    }
}
