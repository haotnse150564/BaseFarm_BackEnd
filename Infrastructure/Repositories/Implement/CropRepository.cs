using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implement
{
    public class CropRepository : GenericRepository<Crop>, ICropRepository
    {
        public CropRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<Crop>();
        }
    }
}
