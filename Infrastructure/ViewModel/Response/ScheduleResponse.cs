using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Response
{
    public class ScheduleResponse
    {
        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public Status? Status { get; set; }

        public DateOnly? CreatedAt { get; set; }

        public DateOnly? UpdatedAt { get; set; }

        public long AssignedTo { get; set; }

        public int FarmActivityId { get; set; }

        public long FarmDetailsId { get; set; }
    }
}
