using Application.Repositories;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IIOTLogRepository : IGenericRepository<IOTLog>
    {
        void DeleteRange(List<IOTLog> entities);
        Task<IOTLog?> GetLatestByPinAsync(string pin);
    }
}
