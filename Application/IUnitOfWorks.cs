

using Infrastructure.Repositories;

namespace Application
{
    public interface IUnitOfWorks : IDisposable
    {
        IProductRepository productRepository { get; }
    }
}
