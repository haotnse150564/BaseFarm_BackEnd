using Application.Repositories;
using Domain.Model;

namespace Infrastructure.Repositories
{
    public interface IAccountRepository : IGenericRepository<Account>
    {
        Task<Account?> GetByEmailAsync(string email);
    }
}
