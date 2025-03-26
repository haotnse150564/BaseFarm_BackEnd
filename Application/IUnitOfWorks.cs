

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
    }
}
