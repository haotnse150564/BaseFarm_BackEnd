using Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Request
{
    public class DailyLogRequest
    {
        public class DailyLogForm {
            [Required(ErrorMessage = "Date is required.")]
            public DateOnly? Date { get; set; }

            [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
            public string? Notes { get; set; }

            [Required(ErrorMessage = "Status is required.")]
            public Status? Status { get; set; }

            [Required(ErrorMessage = "CreatedAt is required.")]
            public DateOnly? CreatedAt { get; set; }

            public DateOnly? UpdatedAt { get; set; }

            [Required(ErrorMessage = "AssignedTo is required.")]
            [Range(1, long.MaxValue, ErrorMessage = "AssignedTo must be a positive number.")]
            public long AssignedTo { get; set; }
        }
    }
}
