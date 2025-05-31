using Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Request
{
    public class FarmActivityRequest
    {
        [Required(ErrorMessage = "FarmActivityName is required.")]
        public DateOnly? StartDate { get; set; }
        [Required(ErrorMessage = "FarmActivityName is required.")]
        public DateOnly? EndDate { get; set; }
    }
}
