using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Request
{
    public class CropRequest
    {
        public string? CropName { get; set; }

        public string? Description { get; set; }


        public string? ImageUrl { get; set; }

        public string? Origin { get; set; }


        public long CategoryId { get; set; }


    }
}
