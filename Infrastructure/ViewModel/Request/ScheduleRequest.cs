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
        [Required(ErrorMessage = "StartDate is required.")]
        public DateOnly? StartDate { get; set; }

        [Required(ErrorMessage = "EndDate is required.")]
        public DateOnly? EndDate { get; set; }

        [Required(ErrorMessage = "AssignedTo is required.")]
        [Range(1, long.MaxValue, ErrorMessage = "AssignedTo must be a positive number.")]
        public long AssignedTo { get; set; }

        [Required(ErrorMessage = "FarmActivityId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "FarmActivityId must be a positive number.")]
        public int FarmActivityId { get; set; }

        [Required(ErrorMessage = "FarmDetailsId is required.")]
        [Range(1, long.MaxValue, ErrorMessage = "FarmDetailsId must be a positive number.")]
        public long FarmDetailsId { get; set; }

        [Required(ErrorMessage = "CropId is required.")]
        [Range(1, long.MaxValue, ErrorMessage = "CropId must be a positive number.")]
        public long CropId { get; set; }

        [Required(ErrorMessage = "PlantingDate is required.")]
        [Range(1, long.MaxValue, ErrorMessage = "CropId must be a positive number.")]
        public DateOnly? PlantingDate { get; set; }

        [Required(ErrorMessage = "HarvestDate is required.")]
        [Range(1, long.MaxValue, ErrorMessage = "CropId must be a positive number.")]
        public DateOnly? HarvestDate { get; set; }
        //public long DailyLogId { get; set; }

    }
}
