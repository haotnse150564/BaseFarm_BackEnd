using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Request
{
    public class AutoConfig
    {
        // Ngưỡng LDR để xử lý AUTO
        public int LdrLow { get; set; } = 300;   
        public int LdrHigh { get; set; } = 600;  

        // Ngưỡng đất cho bơm
        public int SoilOn { get; set; } = 10;   
        public int SoilOff { get; set; } = 30;   
    }
}
