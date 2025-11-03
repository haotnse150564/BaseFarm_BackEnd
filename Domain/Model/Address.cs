using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class Address
    {
        public long AddressID { get; set; }
        public long CustomerID { get; set; }
        public string CustomerName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Province { get; set; }
       // public string District { get; set; }
        public string Ward { get; set; }
        public string Street { get; set; }
        public bool IsDefault { get; set; } = false;
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Optional: Navigation property
        public Account? Account { get; set; }
    }
}
