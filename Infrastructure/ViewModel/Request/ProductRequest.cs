using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.Request
{
    public class ProductRequestDTO
    {
        public class CreateProductDTO
        {
            [Required]
            [StringLength(255, ErrorMessage = "Product Name cannot exceed 255 characters.")]
            public string? ProductName { get; set; }

            [Required(ErrorMessage = "Price is required.")]
            [Range(5000.00, double.MaxValue, ErrorMessage = "Price must be greater than 5000.")]
            public decimal? Price { get; set; }
            public string? Images { get; set; }

            //[Required(ErrorMessage = "Stock Quantity is required.")]
            //[Range(0, int.MaxValue, ErrorMessage = "Stock Quantity cannot be negative.")]
            //public int? StockQuantity { get; set; }

            [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
            public string? Description { get; set; }

            //[Required(ErrorMessage = "Category ID is required.")]
            //[Range(1, long.MaxValue, ErrorMessage = "Category ID must be a positive number.")]
            //public long CategoryId { get; set; }

            //[Required(ErrorMessage = "Category ID is required.")]
            //[Range(1, long.MaxValue, ErrorMessage = "Crop ID must be a positive number.")]
            //public long CropId { get; set; }

        }
        public class UpdateProductDTO
        {
            [Required]
            [StringLength(255, ErrorMessage = "Product Name cannot exceed 255 characters.")]
            public string? ProductName { get; set; }

            [Required(ErrorMessage = "Price is required.")]
            [Range(5000.00, double.MaxValue, ErrorMessage = "Price must be greater than 5000.")]
            public decimal? Price { get; set; }
            public string? Images { get; set; }

            //[Required(ErrorMessage = "Stock Quantity is required.")]
            //[Range(0, int.MaxValue, ErrorMessage = "Stock Quantity cannot be negative.")]
            //public int? StockQuantity { get; set; }

            [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
            public string? Description { get; set; }

            [Required(ErrorMessage = "Category ID is required.")]
            [Range(1, long.MaxValue, ErrorMessage = "Category ID must be a positive number.")]

            public long? CategoryId { get; set; }
           // Required(ErrorMessage = "Category ID is required.")]
           // [Range(1, long.MaxValue, ErrorMessage = "Crop ID must be a positive number.")]
           // public long CropId { get; set; }

        }
        public class UpdateQuantityDTO
        {
            [Required(ErrorMessage = "Stock Quantity is required.")]
            [Range(0, int.MaxValue, ErrorMessage = "Stock Quantity cannot be negative.")]
            public int? StockQuantity { get; set; }
        }
    }
}
