using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class Staff_FarmActivity
    {
        public long Staff_FarmActivityId { get; set; }
        public long AccountId { get; set; }
        public long FarmActivityId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Status status { get; set; }
        public IndividualStatus individualStatus { get; set; }
        public virtual Account? Account { get; set; }
        public virtual FarmActivity? FarmActivity { get; set; }

    }
}
