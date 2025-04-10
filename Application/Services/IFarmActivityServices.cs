using Infrastructure.ViewModel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.FarmActivityResponse;

namespace Application.Services
{
    public interface IFarmActivityServices
    {
        Task<List<FarmActivityView>> GetFarmActivitiesAsync();
    }
}
