using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Request
{
    public class CropRequirementRequest
    {
        public int? EstimatedDate { get; set; }
        public decimal? Moisture { get; set; }
        public decimal? Temperature { get; set; }
        public string? Fertilizer { get; set; }
        public decimal? LightRequirement { get; set; }
        public int? WateringFrequency { get; set; }
        public string? Notes { get; set; }
    }
}
