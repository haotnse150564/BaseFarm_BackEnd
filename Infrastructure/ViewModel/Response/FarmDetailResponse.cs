using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Response
{
    public class FarmDetailResponse
    {
        public class FarmDetailView
        {

            public string? FarmName { get; set; }

            public string? Location { get; set; }

            public DateOnly? CreatedAt { get; set; }

            public DateOnly? UpdatedAt { get; set; }
        }
    }
}
