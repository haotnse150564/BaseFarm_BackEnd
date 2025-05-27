using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum
{
    public enum PaymentStatus
    {
        PAID,
        UNDISCHARGED,
        PENDING,
        CANCELLED,
        COMPLETED,
        DELIVERED
    }
}
