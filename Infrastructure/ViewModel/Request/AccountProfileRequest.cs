using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Request
{
    public class AccountProfileRequest
    {
        public class ProfileRequestDTO
        {
           
            [Range(0, 2, ErrorMessage = "Gender must be 0 or 1 or 2")]
            public int? Gender { get; set; }

            [Phone(ErrorMessage = "Invalid phone number format")]
            public string? Phone { get; set; }

            [Required(ErrorMessage = "Fullname is required")]
            [MaxLength(100, ErrorMessage = "Fullname cannot exceed 100 characters")]
            public string? Fullname { get; set; }

            [MaxLength(255, ErrorMessage = "Address cannot exceed 255 characters")]
            public string? Address { get; set; }
            public string? Images { get; set; }
        }
    }
}
