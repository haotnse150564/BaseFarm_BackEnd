using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Response
{
    public class DailyLogResponse
    {
        public class DailyLogView
        {
            public long TrackingId { get; set; }

            public DateOnly? Date { get; set; }

            public string? Notes { get; set; }

            public Status? Status { get; set; }

            public DateOnly? CreatedAt { get; set; }

            public DateOnly? UpdatedAt { get; set; }

            public long AssignedTo { get; set; }
        }
    }
}
