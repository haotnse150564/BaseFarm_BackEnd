using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.AccountResponse;

namespace Infrastructure.ViewModel.Request
{
    public class ScheduleRequest
    {
        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public long AssignedTo { get; set; }

        public int FarmActivityId { get; set; }

        public long FarmDetailsId { get; set; }
        public long CropId { get; set; }
        public long DailyLogId { get; set; }

    }
}
