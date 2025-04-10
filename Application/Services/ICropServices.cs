using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.CropResponse;

namespace Application.Services
{
    public interface ICropServices
    {
        Task<List<CropView>> GetAllCropsAsync();
    }
}
