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
        private IFeedbackRepository _feedbackRepository;
        private IAccountProfileRepository _accountProfileRepository;
        private IAccountRepository _accountRepository;

        public UnitOfWorks(AppDbContext context, ICurrentTime currentTime, IClaimsServices claimsServices, IProductRepository productRepository, IFeedbackRepository feedbackRepository, IAccountProfileRepository accountProfileRepository)
        {
            _context = context;
            _currentTime = currentTime;
            _claimsServices = claimsServices;
            _productRepository = productRepository;
            _feedbackRepository = feedbackRepository;
            _accountProfileRepository = accountProfileRepository;
        }

        public IProductRepository productRepository
        {
            get
            {
                return _productRepository ??= new ProductRepository(_context);
            }
        }

        public IFeedbackRepository feedbackRepository
        {
            get
            {
                return _feedbackRepository ??= new FeedbackRepository(_context);
            }
        }
        public IAccountProfileRepository accountProfileRepository
        {
            get
            {
                return _accountProfileRepository ??= new AccountProfileRepository(_context);
            }
        }

        public IAccountRepository accountRepository
        {
            get
            {
                return _accountRepository ??= new AccountRepository(_context);
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
