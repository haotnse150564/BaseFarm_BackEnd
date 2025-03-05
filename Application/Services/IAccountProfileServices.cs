using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.AccountProfileResponse;

namespace Application.Services
{
    public interface IAccountProfileServices
    {
        Task<ProfileResponseDTO> ViewProfileAsync();
    }
}
