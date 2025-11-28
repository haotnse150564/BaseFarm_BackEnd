using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.IOTLogResponse;

namespace Application.Services
{
    public interface IIOTLogServices
    {
        Task<string> UpdateLogAsync();
        Task<ResponseDTO> GetList();

    }
}
