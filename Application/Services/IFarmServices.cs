using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.FarmDetailResponse;

namespace Application.Services
{
    public interface IFarmServices
    {
        Task<List<FarmDetailView>> GetAll();
    }
}
