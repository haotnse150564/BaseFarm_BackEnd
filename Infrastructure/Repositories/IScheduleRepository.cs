using Application.Repositories;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IScheduleRepository : IGenericRepository<Schedule>
    {
        Task<List<Schedule?>> GetByStaffIdAsync(long staffId, int month);
        Task<List<Schedule?>> GetScheduleByStaffIdAsync(long staffId, int month);
        Task<Schedule?> GetByIdWithFarmActivitiesAsync(long scheduleId);
        Task<Schedule?> GetByCropId(long cropId);

        Task<Schedule?> GetByIdWithCropRequirementsAsync(long scheduleId, long managerId);
    }
}
