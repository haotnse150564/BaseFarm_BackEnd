using Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.AccountResponse;

namespace Infrastructure.ViewModel.Request
{
    public class ScheduleRequest
    {
        [Required]
        public long FarmId { get; set; }

        [Required]
        public long CropId { get; set; }
        [Required]
        public long StaffId { get; set; }
        [Required]
        public DateOnly? StartDate { get; set; }
        [Required]
        public DateOnly? EndDate { get; set; }

        [Required]
        public int? Quantity { get; set; }

        [Required]
        public Status? Status { get; set; }

        public bool PesticideUsed { get; set; }

        public DiseaseStatus? DiseaseStatus { get; set; }
    }

    public class UpdateTodayRequest
    {
        // Nếu null → dùng ngày hiện tại
        // Nếu có giá trị → dùng để demo / backdate
        public DateOnly? CustomToday { get; set; } = null;
    }

    public class ScheduleLogDto
    {
        public long CropLogId { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public long CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public long UpdatedBy { get; set; }
    }

    public class CreateScheduleLogRequest
    {
        public long ScheduleId { get; set; }
        public string Notes { get; set; } = string.Empty;
    }

    public class UpdateScheduleLogRequest
    {
        public long LogId { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
