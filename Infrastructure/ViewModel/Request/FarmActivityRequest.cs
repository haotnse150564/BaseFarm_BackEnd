using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Request
{
    public class FarmActivityRequest
    {

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public long ScheduleId { get; set; }
    }
}
