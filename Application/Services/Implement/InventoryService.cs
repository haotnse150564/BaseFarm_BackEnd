using Domain.Enum;
using Domain.Model;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Implement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.ViewModel.Response.OrderResponse;

namespace Application.Services.Implement
{
    public class InventoryService : IInventoryService
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IUnitOfWorks _unitOfWork;

        public InventoryService(IScheduleRepository scheduleRepository, IInventoryRepository inventoryRepository, IUnitOfWorks unitOfWork)
        {
            _scheduleRepository = scheduleRepository;
            _inventoryRepository = inventoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseDTO> CalculateAndCreateInventoryAsync(int? quantity, string? location, long productID, long scheduleID)
        {
            // Tính sản lượng
            if (quantity == null)
                throw new Exception("Schedule Quantity is null");
            var stockQuantity = (int)(quantity * 0.8);

            var inventory = new Inventory
            {
                Location = location,
                StockQuantity = stockQuantity,
                CreatedAt = DateOnly.FromDateTime(DateTime.Now),
                Status = Status.ACTIVE,
                ProductId = productID, 
                ScheduleId = scheduleID,
                ExpiryDate = null, 
            };

            await _inventoryRepository.AddAsync(inventory);
            return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG);
        }

        public async Task SyncProductStockQuantityAsync(long productId)
        {
            var totalStock = await _inventoryRepository.GetTotalStockByProductIdAsync(productId);
            var product = await _unitOfWork.productRepository.GetByIdAsync(productId);
            if (product != null)
            {
                product.StockQuantity = totalStock;
                await _unitOfWork.productRepository.UpdateAsync(product);
            }
        }
    }
}
