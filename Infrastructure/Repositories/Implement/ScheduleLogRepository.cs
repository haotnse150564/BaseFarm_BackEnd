using Application.Repositories;
using Domain.Model;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.ScheduleResponse;

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
        public async Task<List<ScheduleLogResponse>> GetAllWithName(long scheduleId)
        {
            var query = from log in _context.ScheduleLog
                        join createBy in _context.AccountProfile
                            on log.CreatedBy equals createBy.AccountProfileId into createByJoin
                        from createBy in createByJoin.DefaultIfEmpty() // left join
                        join updateBy in _context.AccountProfile
                            on log.UpdatedBy equals updateBy.AccountProfileId into updateByJoin
                        from updateBy in updateByJoin.DefaultIfEmpty()
                        where log.ScheduleId == scheduleId
                        orderby log.CreatedAt descending
                        select new ScheduleLogResponse
                        {
                            CropLogId = log.CropLogId,
                            Notes = log.Notes,
                            CreatedAt = log.CreatedAt,
                            CreateBy = createBy.Fullname != null ? createBy.Fullname : null,
                            UpdatedAt = log.UpdatedAt,
                            UpdateBy = updateBy.Fullname != null ? updateBy.Fullname : null
                        };

            return await query.AsNoTracking().ToListAsync();
        }

    }
}
