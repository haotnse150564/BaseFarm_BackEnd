using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IIOTLogServices
    {
        Task<IOTLog> CreateLogAsync(IOTLog log);
        Task<IOTLog> UpdateLogAsync(IOTLog log);
        Task<IOTLog> GetLogByIdAsync(Guid id);

    }
}
