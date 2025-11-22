using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class IOTLog
    {
        public long IotLogId { get; set; } 

        public long DevicesId { get; set; } 

        public string? VariableId { get; set; } 

        public string? SensorName { get; set; } 

        public double? Value { get; set; } 

        public DateTime Timestamp { get; set; } 

        public virtual Device Device { get; set; } = null!;
    
    }
}
