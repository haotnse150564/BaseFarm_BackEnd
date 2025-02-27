

using Infrastructure.Repositories;

namespace Application
{
    public interface IUnitOfWorks : IDisposable
    {
        Task<int> SaveChangesAsync();
        IProductRepository productRepository { get; }
        IFeedbackRepository feedbackRepository { get; }
    }
}
