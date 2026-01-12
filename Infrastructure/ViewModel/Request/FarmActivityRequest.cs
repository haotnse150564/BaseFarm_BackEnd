using Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Request
{
    public class FarmActivityRequest
    {
        [Required(ErrorMessage = "FarmActivity Start Date is required.")]
        public DateOnly? StartDate { get; set; }
        [Required(ErrorMessage = "FarmActivity End Date is required.")]
        public DateOnly? EndDate { get; set; }
        [Required(ErrorMessage = "Staff is required.")]
       // public long StaffId { get; set; }
        public long? ScheduleId { get; set; }

    }
}
