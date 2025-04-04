using Application.Commons;
using Infrastructure.ViewModel.Request;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Request.AccountRequest;
using static Infrastructure.ViewModel.Response.AccountResponse;

namespace Application.Services
{
    public interface IAccountServices
    {
        Task<LoginResponseDTO> LoginAsync(string email, string password);
        Task<ResponseDTO> RegisterAsync(RegisterRequestDTO request);
        Task<ViewAccount> UpdateAccountStatusAsync(long id);
        Task<ViewAccount> CreateAccountAsync(AccountForm request);
        Task<ViewAccount> UpdateAccountAsync(long id, AccountForm request);
        Task<Pagination<ViewAccount>> GetAllAccountAsync(int pageSize, int pageIndex );

    }
}
