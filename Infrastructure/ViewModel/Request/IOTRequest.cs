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

        //[Required(ErrorMessage = "FarmActivityName is required.")]
        public string? DeviceName { get; set; }

        //[Required(ErrorMessage = "FarmActivityName is required.")]
        public string? DeviceType { get; set; }

        //[Required(ErrorMessage = "FarmActivityName is required.")]
        public DateOnly? ExpiryDate { get; set; }

        //[Required(ErrorMessage = "FarmActivityName is required.")]
        //public long FarmDetailsId { get; set; }
    }
}
