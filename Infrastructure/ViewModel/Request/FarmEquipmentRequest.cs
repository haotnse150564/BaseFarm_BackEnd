using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Request
{
    public class FarmEquipmentRequest
    {
        public long deviceId { get; set; }
        public string? Note { get; set; }
    }
}
