using Application.Repositories;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface ICropRepository : IGenericRepository<Crop>
    {
        Task<bool> CheckDuplicateCropName(string cropName);
        Task<List<Crop>> GetAllAsync();
        Task<List<Crop>> GetAllexcludingInactiveAsync();
    }
}
