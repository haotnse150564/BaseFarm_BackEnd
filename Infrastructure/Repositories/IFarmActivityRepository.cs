using Application.Repositories;
using Domain.Enum;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IFarmActivityRepository : IGenericRepository<FarmActivity>
    {
        Task<List<FarmActivity>>GetAllActive();
        Task<FarmActivity> GetHarvestFarmActivityId(long scheduleId);
        Task<List<FarmActivity>> GetListFarmActivityByScheduleId(long scheduleId);
        Task<List<FarmActivity>> GetListFarmActivityUpdate(IEnumerable<long>? farmActivityId);
        Task<List<FarmActivity>> GetAllFiler(Domain.Enum.ActivityType? type, Domain.Enum.FarmActivityStatus? status, int? month);
        Task<List<Product>> GetProductWillHarves(long acitivityId);
        Task<bool> HasDuplicateActivityTypeInScheduleAsync(long scheduleId, ActivityType activityType, long? excludeActivityId = null);
        Task<bool> HasOverlappingActiveActivityAsync(long? scheduleId, DateOnly startDate, DateOnly endDate, long excludeActivityId);
        Task<bool> HasOverlappingActivityInScheduleAsync(long scheduleId, DateOnly startDate, DateOnly endDate, long? excludeActivityId = null);
        Task<List<Crop>> GetCropsForHarvestActivity(long activityId);
        Task<List<Staff_FarmActivity>> GetAssignStaffByFarmActivityIdAndAccountId(long farmActivityId, long accountId);
    }
}
