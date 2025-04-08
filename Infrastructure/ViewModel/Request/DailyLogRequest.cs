using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Request
{
    public class DailyLogRequest
    {
        public class DailyLogForm {
            public DateOnly? Date { get; set; }

            public string? Notes { get; set; }

            public Status? Status { get; set; }

            public DateOnly? CreatedAt { get; set; }

            public DateOnly? UpdatedAt { get; set; }

            public long AssignedTo { get; set; } 
        }
    }
}
