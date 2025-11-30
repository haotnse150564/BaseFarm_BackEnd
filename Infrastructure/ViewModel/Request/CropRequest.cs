using Application.ViewModel.Request;
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
        [Required(ErrorMessage = "Origin is required.")]
        public string? Origin { get; set; }
        [Required(ErrorMessage = "Category is required.")]
        public long? CategoryId { get; set; }
        public string? Images { get; set; }

    }
    public class Crop_Product
    {
        public CropRequest request1 { get; set; }
        public ProductRequestDTO.CreateProductDTO request2 { get; set; }
    }
}
