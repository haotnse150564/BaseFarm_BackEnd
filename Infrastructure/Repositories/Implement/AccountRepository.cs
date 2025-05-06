using Domain.Enum;
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

        public async Task<List<Account>> GetAllAccountWithProfiles(Status? status, Roles? role)
        {
            var query = _dbSet.Include(x => x.AccountProfile).AsQueryable();

            // Nếu có trạng thái, lọc theo trạng thái
            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status);
            }

            // Nếu có role, lọc theo role
            if (role.HasValue)
            {
                query = query.Where(x => x.Role == role);
            }

            // Thực thi truy vấn và trả về danh sách tài khoản
            var result = await query.ToListAsync();
            return result;
        }


        public async Task<Account> GetByEmail(string email)
        {
            var result = await _dbSet.Where(x => x.Email == email)
                           .Include(c => c.AccountProfile)
                           .FirstOrDefaultAsync();
            return result;
        }
        public override async Task<Account?> GetByIdAsync(long id)
        {
            return await _dbSet.Include(x => x.AccountProfile).FirstOrDefaultAsync(x => x.AccountId == id);
        }
    }
}
