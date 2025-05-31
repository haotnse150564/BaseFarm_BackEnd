using Application.Repositories;
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
    }
}
