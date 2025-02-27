using Application;
using Application.Interfaces;
using Application.Services;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Implement;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class UnitOfWorks : IUnitOfWorks
    {
        private AppDbContext _context;
        private ICurrentTime _currentTime;
        private IClaimsServices _claimsServices;
        private IProductRepository _productRepository;

        public UnitOfWorks(AppDbContext context, ICurrentTime currentTime, IClaimsServices claimsServices, IProductRepository productRepository)
        {
            _context = context;
            _currentTime = currentTime;
            _claimsServices = claimsServices;
            _productRepository = productRepository;
        }

        public IProductRepository productRepository
        {
            get
            {
                return _productRepository ??= new ProductRepository(_context);
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
