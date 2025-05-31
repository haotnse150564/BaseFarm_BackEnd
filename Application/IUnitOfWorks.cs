

using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Application
{
    public interface IUnitOfWorks : IDisposable
    {
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<int> SaveChangesAsync();
        IProductRepository productRepository { get; }
        IFeedbackRepository feedbackRepository { get; }
        IAccountProfileRepository accountProfileRepository { get; }
        IAccountRepository accountRepository { get; }
        IOrderRepository orderRepository { get; }
        IOrderDetailRepository orderDetailRepository { get; }
        IPaymentRepository paymentRepository { get; }
        IScheduleRepository scheduleRepository { get; }
        IIoTdeviceRepository ioTdeviceRepository { get; }
        ICropRepository cropRepository { get; }
        ICropRequirementRepository cropRequirementRepository { get; }
        IFarmActivityRepository farmActivityRepository { get; }
        IFarmRepository farmRepository { get; }
        ICategoryRepository categoryRepository { get; }
        IInventoryRepository inventoryRepository { get; }

    }
}
