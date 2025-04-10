using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Response
{
    public class CropResponse
    {
        public class CropView
        {
            public string? CropName { get; set; }

            public string? Description { get; set; }

            public int? Quantity { get; set; }

            public string Status { get; set; }

            public DateOnly? PlantingDate { get; set; }

            public DateOnly? HarvestDate { get; set; }
        }

    }
}
