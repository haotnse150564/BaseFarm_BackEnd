using Domain.Enum;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class FarmEquipment
    {
        public long FarmEquipmentId { get; set; }
        public DateOnly AssignDate { get; set; }
        public long RemoveDate { get; set; }
        public Status Status { get; set; }
        public string? Note { get; set; }
        public long FarmId { get; set; }
        public long DeviceId { get; set; }

        public virtual Farm? Farm { get; set; }
        public virtual Device? Device { get; set; }
    }
}
