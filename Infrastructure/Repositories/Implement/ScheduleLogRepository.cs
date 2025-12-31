using Application.Repositories;
using Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implement
{
    public class ScheduleLogRepository : GenericRepository<ScheduleLog>, IScheduleLogRepository
    {
        public ScheduleLogRepository(AppDbContext context)
        {
            {
                _context = context;
                _dbSet = _context.Set<ScheduleLog>();
            }
        }

        public async Task<List<ScheduleLog>> GetAllByScheduleIdAsync(long scheduleId)
        {
            return await _context.ScheduleLog
                .Where(log => log.ScheduleId == scheduleId)
                .OrderByDescending(log => log.CreatedAt)  // Mới nhất trước
                .AsNoTracking()
                .ToListAsync();
        }
    }   
}
