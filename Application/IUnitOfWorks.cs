

using Infrastructure.Repositories;

namespace Application
{
    public interface IUnitOfWorks : IDisposable
    {
        Task<int> SaveChangesAsync();
        IProductRepository productRepository { get; }
        IFeedbackRepository feedbackRepository { get; }
        IAccountProfileRepository accountProfileRepository { get; }
        IAccountRepository accountRepository { get; }
        IOrderRepository orderRepository { get; }
        IOrderDetailRepository orderDetailRepository { get; }
    }
}
