using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.Response
{
    public class CartItemResponse
    {
        public long CartId { get; set; }
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal PriceQuantity { get; set; }
    }
}
