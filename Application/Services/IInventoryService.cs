using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.ViewModel.Response.OrderResponse;

namespace Application.Services
{
    public interface IInventoryService
    {
        Task<ResponseDTO> CalculateAndCreateInventoryAsync(int? quantity, string? location, long productID, long scheduleID);
        Task SyncProductStockQuantityAsync(long productId);
    }
}
