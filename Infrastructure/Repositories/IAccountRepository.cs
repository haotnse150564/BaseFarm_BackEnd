using Application.Repositories;
using Domain.Enum;
using Domain.Model;

namespace Infrastructure.Repositories
{
    public interface IAccountRepository : IGenericRepository<Account>
    {
        Task<Account?> GetByEmailAsync(string email);
        Task<Account> GetAccountProfileByAccountIdAsync(long accountID);
        Task<List<Account>> GetAllAccountWithProfiles(AccountStatus? status, Roles? role);
        Task<Account> GetByEmail(string email);

        Task<List<Account>> GetAvailableStaffAsync();

    }
}
