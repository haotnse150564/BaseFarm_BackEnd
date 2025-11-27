using Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
        public DateOnly? PlantingDate { get; set; }
        //[Required]
        //public DateOnly? HarvestDate { get; set; }
        [Required]
        public int? Quantity { get; set; }

        [Required]
        public Status? Status { get; set; }

        public bool PesticideUsed { get; set; }

        public DiseaseStatus? DiseaseStatus { get; set; }

        public long FarmActivitiesId { get; set; }
    }
}
