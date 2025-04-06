using Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Infrastructure.Repositories.Implement
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        private readonly AppDbContext _context;

        public AccountRepository(AppDbContext context) => _context = context;

        public async Task<Account?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(a => a.Email == email);
        }
        public async Task<Account> GetAccountProfileByAccountIdAsync(long accountID)
        {
            var result = await _dbSet.Where(x => x.AccountId == accountID).Include(c => c.AccountProfile).FirstOrDefaultAsync();
            return result;
        }

        public async Task<List<Account>> GetAllAccountWithProfiles()
        {
            var result = await _dbSet.Include(x => x.AccountProfile).ToListAsync();
            return result;
        }

        public async Task<Account> GetByEmail(string email)
        {
            var result = await _dbSet.Where(x => x.Email == email)
                           .Include(c => c.AccountProfile)
                           .FirstOrDefaultAsync();
            return result;
        }
    }
}
