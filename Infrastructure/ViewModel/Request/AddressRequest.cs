using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Request
{
    public class AddressRequest
    {
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
        public string? Street { get; set; }
        public bool IsDefault { get; set; } = false;
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }
}
