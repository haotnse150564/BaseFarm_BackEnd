using Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Request
{
    public class IOTRequest
    {

        [Required(ErrorMessage = "Devices Name is required.")]
        public string? DeviceName { get; set; }

        [Required(ErrorMessage = "Device Type is required.")]
        public string? DeviceType { get; set; }

        [Required(ErrorMessage = "Expiry Date is required.")]
        public DateOnly? ExpiryDate { get; set; }

        [Required(ErrorMessage = "Farm Details is required.")]
        [Range(1, long.MaxValue, ErrorMessage = "Farm Details Id must be a positive number.")]
        public long FarmDetailsId { get; set; }
    }
}
