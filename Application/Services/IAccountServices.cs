using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.AccountResponse;

namespace Application.Services
{
    public interface IAccountServices
    {
        Task<LoginResponseDTO> LoginAsync(int phone, string password);
    }
}
