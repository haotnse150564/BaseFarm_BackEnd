using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Response
{
    public class FarmDetailResponse
    {
        public class FarmView
        {
            public long? FarmId { get; set; }
            public string? FarmName { get; set; }

            public string? Location { get; set; }

            public string? CreatedAt { get; set; }

            public string? UpdatedAt { get; set; }
        }
    }
}
