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
        Task<List<Schedule?>> GetByStaffIdAsync(long staffId);
    }
}
