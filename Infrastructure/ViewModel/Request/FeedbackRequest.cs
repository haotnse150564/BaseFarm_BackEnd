using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Request
{
    public class FeedbackRequest
    {
        public class CreateFeedbackDTO
        {
            [Required(ErrorMessage = "Comment is required")]
            public string? Comment { get; set; }

            [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
            public int? Rating { get; set; }

            [Required(ErrorMessage = "CustomerId is required")]
            public long CustomerId { get; set; }

        }
    }
}
