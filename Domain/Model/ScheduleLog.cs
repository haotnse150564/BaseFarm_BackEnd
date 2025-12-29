using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class ScheduleLog
    {
        public long CropLogId { get; set; }
        public long ScheduleId { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public long CreatedBy { get; set; }
        public long UpdatedBy { get; set; }
        // Navigation property
        public virtual Schedule? schedule { get; set; }
    }
}
