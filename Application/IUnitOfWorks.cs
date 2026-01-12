

using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Application
{
    public interface IUnitOfWorks : IDisposable
    {
        Task<IDbContextTransaction> BeginTransactionAsync();
        DatabaseFacade Database { get; }
        Task<int> SaveChangesAsync();
        IProductRepository productRepository { get; }
        IFeedbackRepository feedbackRepository { get; }
        IAccountProfileRepository accountProfileRepository { get; }
        IAccountRepository accountRepository { get; }
        IOrderRepository orderRepository { get; }
        IOrderDetailRepository orderDetailRepository { get; }
        IPaymentRepository paymentRepository { get; }
        IScheduleRepository scheduleRepository { get; }
        IDevicesRepository deviceRepository { get; }
        ICropRepository cropRepository { get; }
        ICropRequirementRepository cropRequirementRepository { get; }
        IFarmActivityRepository farmActivityRepository { get; }
        IFarmRepository farmRepository { get; }
        ICategoryRepository categoryRepository { get; }
        IInventoryRepository inventoryRepository { get; }
        ICartRepository cartRepository { get; }
        ICartItemRepository cartItemRepository { get; }
        IAddressRepository addressRepository { get; }
        IIOTLogRepository iotLogRepository { get; }
        IFarmEquipmentRepository farmEquipmentRepository { get; }
        IScheduleLogRepository scheduleLogRepository { get; }
        IStaff_FarmActivityRepository staff_FarmActivityRepository { get; }
    }
}
