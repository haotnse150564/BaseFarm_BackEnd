using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.Request
{
    public class OrderRequest
    {

        public class SelectProductDTO
        {
            public long ProductId { get; set; }
            public int? StockQuantity { get; set; }
        }

        public class CreateOrderDTO
        {
            public List<SelectProductDTO> OrderItems { get; set; } = new();
        }
    }
}
