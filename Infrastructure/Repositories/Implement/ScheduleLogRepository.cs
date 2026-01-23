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
                        where log.ScheduleId == scheduleId
                        // Left join với AccountProfile cho người tạo
                        join createProfile in _context.AccountProfile
                            on log.CreatedBy equals createProfile.AccountProfileId into createJoin
                        from createProfile in createJoin.DefaultIfEmpty()
                            // Left join với AccountProfile cho người cập nhật
                        join updateProfile in _context.AccountProfile
                            on log.UpdatedBy equals updateProfile.AccountProfileId into updateJoin
                        from updateProfile in updateJoin.DefaultIfEmpty()
                        orderby log.CreatedAt descending
                        select new ScheduleLogResponse
                        {
                            CropLogId = log.ScheduleLogId,                   
                            FarmActivityId = log.FarmActivityId,              
                            Notes = log.Notes,
                            CreatedAt = log.CreatedAt,       
                            UpdatedAt = log.UpdatedAt,
                            CreateBy = createProfile != null
                                ? (createProfile.Fullname ?? "Unknown")
                                : "Hệ thống",

                            UpdateBy = updateProfile != null
                                ? (updateProfile.Fullname ??  "Unknown")
                                : "Hệ thống",
                        };

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<bool> ExistsTodayAsync(long farmActivityId, long scheduleId, DateTime date)
        {
            // Giữ nguyên nếu bạn vẫn cần method cũ (không theo user)
            return await _context.ScheduleLog
                .AnyAsync(s =>
                    s.FarmActivityId == farmActivityId &&
                    s.ScheduleId == scheduleId &&
                    s.CreatedAt.Date == date.Date);
        }

        public async Task<int> CountTodayByUserAsync(long farmActivityId, long scheduleId, DateTime date, long userId)
        {
            return await _context.ScheduleLog
                .CountAsync(s =>
                    s.FarmActivityId == farmActivityId &&
                    s.ScheduleId == scheduleId &&
                    s.CreatedAt.Date == date.Date &&
                    s.CreatedBy == userId);
        }

        public async Task<ScheduleLog?> GetLatestTodayByUserAsync(long farmActivityId, long scheduleId, DateTime date, long userId)
        {
            return await _context.ScheduleLog
                .AsNoTracking()
                .Where(s =>
                    s.FarmActivityId == farmActivityId &&
                    s.ScheduleId == scheduleId &&
                    s.CreatedAt.Date == date.Date &&
                    s.CreatedBy == userId)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync();
        }
    }
}
