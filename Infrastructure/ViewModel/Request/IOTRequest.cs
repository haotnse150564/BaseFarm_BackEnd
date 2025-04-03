using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Request
{
    public class IOTRequest
    {
        public string? DeviceName { get; set; }

        public string? DeviceType { get; set; }

        public string? SensorValue { get; set; }

        public string? Unit { get; set; }

        public DateOnly? ExpiryDate { get; set; }

        public long FarmDetailsId { get; set; }
    }
}
