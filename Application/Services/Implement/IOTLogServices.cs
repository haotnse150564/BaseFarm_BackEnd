using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Implement
{
    public class IOTLogServices : IIOTLogServices
    {
        public Task<IOTLog> CreateLogAsync(IOTLog log)
        {
            throw new NotImplementedException();
        }

        public Task<IOTLog> GetLogByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<IOTLog> LoadLog()
        {
            throw new NotImplementedException();
        }

        public Task<IOTLog> UpdateLogAsync(IOTLog log)
        {
            throw new NotImplementedException();
        }
    }
}
