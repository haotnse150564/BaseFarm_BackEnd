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

        public async Task<List<Account>> GetAllAccountWithProfiles(AccountStatus? status, Roles? role)
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

        public async Task<List<Account>> GetAvailableStaffAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            // Lấy tất cả staff có Role = Staff
            var allStaffQuery = _context.Account
                .Where(a => a.Role == Roles.Staff && a.Status == AccountStatus.ACTIVE)
                .Include(a => a.AccountProfile); // Để lấy FullName, Phone

            // Lấy danh sách StaffId đang bận (có lịch ACTIVE hôm nay)
            var busyStaffIds = await _context.Schedule
                .Where(s => s.Status == Status.ACTIVE)
                .Select(s => s.AssignedTo)
                .Distinct()
                .ToListAsync();

            // Lọc ra staff không có trong danh sách bận
            var availableStaff = await allStaffQuery
                .Where(a => !busyStaffIds.Contains(a.AccountId))
                .ToListAsync();

            return availableStaff;
        }
    }
}
