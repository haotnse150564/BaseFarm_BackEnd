using Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Request
{
    public class CropRequest
    {
        [Required(ErrorMessage = "CropName is required.")]
        [MaxLength(100, ErrorMessage = "CropName cannot exceed 32 characters.")]
        public string? CropName { get; set; }

        public string? Description { get; set; }
        public string? Origin { get; set; }


    }
}
