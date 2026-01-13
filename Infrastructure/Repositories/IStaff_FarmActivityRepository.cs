using Application.Repositories;
using Domain.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IStaff_FarmActivityRepository : IGenericRepository<Staff_FarmActivity>
    {
        Task<List<Staff_FarmActivity>> GetByStaffIdAsync(long staffId);
        Task<List<Staff_FarmActivity>> GetByFarmActivityIdAsync(long staffId);

    }
}
