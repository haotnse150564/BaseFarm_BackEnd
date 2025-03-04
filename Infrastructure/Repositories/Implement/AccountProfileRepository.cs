using Domain.Model;

namespace Infrastructure.Repositories.Implement
{
    public class AccountProfileRepository : GenericRepository<AccountProfile>, IAccountProfileRepository
    {
        private readonly AppDbContext _context;

        public AccountProfileRepository(AppDbContext context) => _context = context;
    }
}
