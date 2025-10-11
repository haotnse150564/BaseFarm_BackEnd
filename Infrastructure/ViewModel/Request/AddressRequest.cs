using Domain.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Request
{
    public class AddressRequest
    {
        [Required]
        public string CustomerName { get; set; } 
        [Required]
        public string PhoneNumber { get; set; } 
        [Required]
        public string Province { get; set; } 
        [Required]
        public string District { get; set; }    
        [Required]
        public string Ward { get; set; }
        [Required]
        public string Street { get; set; }
        public bool IsDefault { get; set; } = false;
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }
}
