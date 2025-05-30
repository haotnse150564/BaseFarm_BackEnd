using Domain.Enum;
using Infrastructure.ViewModel.Request;
using Infrastructure.ViewModel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.FarmActivityResponse;

namespace Application.Services
{
    public interface IFarmActivityServices
    {
        Task<ResponseDTO> GetFarmActivitiesAsync(int pageIndex, int pageSize);
        Task<ResponseDTO> GetFarmActivitiesActiveAsync(int pageIndex, int pageSize);
        Task<ResponseDTO> CreateFarmActivityAsync(FarmActivityRequest farmActivityRequest, ActivityType activityType);
        Task<ResponseDTO> UpdateFarmActivityAsync(long farmActivityId, FarmActivityRequest farmActivityRequest, ActivityType? activityType);
        Task<ResponseDTO> GetFarmActivityByIdAsync(long farmActivityId);
        Task<ResponseDTO> ChangeFarmActivityStatusAsync(long farmActivityId);

    }
}
