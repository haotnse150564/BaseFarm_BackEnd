using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implement
{
    public class AccountProfileRepository : GenericRepository<AccountProfile>, IAccountProfileRepository
    {
        private readonly AppDbContext _context;

        public AccountProfileRepository(AppDbContext context) => _context = context;
    }
}
