using Application;
using Application.Interfaces;
using Application.Services;
using Domain.Model;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Implement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

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
        private IOrderRepository _orderRepository;
        private IOrderDetailRepository _orderDetailRepository;
        private IPaymentRepository _paymentRepository;
        private IScheduleRepository _scheduleRepository;
        private IDevicesRepository _deviceRepository;
        private ICropRepository _cropRepository;
        private IFarmActivityRepository _farmActivityRepository;
        private IFarmRepository _farmRepository;
        private ICategoryRepository _categoryRepository;
        private IInventoryRepository _inventoryRepository;
        private ICropRequirementRepository _cropRequirementRepository;    
        private ICartRepository _cartRepository;
        private ICartItemRepository _cartItemRepository;
        private IAddressRepository _addressRepository;
        private IIOTLogRepository _iotLogRepository;
        private IFarmEquipmentRepository _farmEquipmentRepository;
        private IScheduleLogRepository _scheduleLogRepository;
        private IStaff_FarmActivityRepository _staff_FarmActivityRepository;

        public DatabaseFacade Database => _context.Database;
        public UnitOfWorks(AppDbContext context, ICurrentTime currentTime, IClaimsServices claimsServices, IProductRepository productRepository
            , IFeedbackRepository feedbackRepository, IAccountProfileRepository accountProfileRepository, IOrderRepository orderRepository
            , IOrderDetailRepository orderDetailRepository, IPaymentRepository paymentRepository, IScheduleRepository scheduleRepository
            , IDevicesRepository deviceRepository, ICropRepository cropRepository, IFarmActivityRepository farmActivityRepository, IFarmRepository farmRepository
            , ICategoryRepository categoryRepository, IAccountRepository accountRepository, IInventoryRepository inventoryRepository
            , ICropRequirementRepository cropRequirementRepository, ICartRepository cartRepository, ICartItemRepository cartItemRepository
            , IAddressRepository addressRepository, IIOTLogRepository iotLogRepository, IFarmEquipmentRepository farmEquipmentRepository
            , IScheduleLogRepository scheduleLogRepository, IStaff_FarmActivityRepository staff_FarmActivityRepository

            )
        {
            _context = context;
            _currentTime = currentTime;
            _claimsServices = claimsServices;
            _productRepository = productRepository;
            _feedbackRepository = feedbackRepository;
            _accountProfileRepository = accountProfileRepository;
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _paymentRepository = paymentRepository;
            _scheduleRepository = scheduleRepository;
            _deviceRepository = deviceRepository;
            _cropRepository = cropRepository;
            _farmActivityRepository = farmActivityRepository;
            _farmRepository = farmRepository;
            _categoryRepository = categoryRepository;
            _accountRepository = accountRepository;
            _inventoryRepository = inventoryRepository;
            _cropRequirementRepository = cropRequirementRepository;
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _addressRepository = addressRepository;
            _addressRepository = addressRepository;
            _iotLogRepository = iotLogRepository;
            _farmEquipmentRepository = farmEquipmentRepository;
            _scheduleLogRepository = scheduleLogRepository;
            _scheduleLogRepository = scheduleLogRepository;
            _staff_FarmActivityRepository = staff_FarmActivityRepository;
        }

        public IStaff_FarmActivityRepository staff_FarmActivityRepository
        {
            get
            {
                return _staff_FarmActivityRepository ??= new Staff_FarmActivityRepository(_context);
            }
        }
        public IScheduleLogRepository scheduleLogRepository
        {
            get
            {
                return _scheduleLogRepository ??= new ScheduleLogRepository(_context);
            }
        }
        public IFarmEquipmentRepository farmEquipmentRepository
        {
            get
            {
                return _farmEquipmentRepository ??= new FarmEquipmentRepository(_context);
            }
        }
        public IIOTLogRepository iotLogRepository
        {
            get
            {
                return _iotLogRepository ??= new IOTLogRepository(_context);
            }
        }
        public IAddressRepository addressRepository
        {
            get
            {
                return _addressRepository ??= new AddressRepository(_context);
            }
        }
        public ICartItemRepository cartItemRepository
        {
            get
            {
                return _cartItemRepository ??= new CartItemRepository(_context);
            }
        }
        public ICartRepository cartRepository
        {
            get
            {
                return _cartRepository ??= new CartRepository(_context);
            }
        }
        public ICropRequirementRepository cropRequirementRepository
        {
            get
            {
                return _cropRequirementRepository ??= new CropRequirementtRepository(_context);
            }
        }
        public ICategoryRepository categoryRepository
        {
            get
            {
                return _categoryRepository ??= new CategoryRepository(_context);
            }
        }
        public ICropRepository cropRepository
        {
            get
            {
                return _cropRepository ??= new CropRepository(_context);
            }
        }
        public IFarmActivityRepository farmActivityRepository
        {
            get
            {
                return _farmActivityRepository ??= new FarmActivityRepository(_context);
            }
        }
        public IFarmRepository farmRepository
        {
            get
            {
                return _farmRepository ??= new FarmRepository(_context);
            }
        }
        public IDevicesRepository deviceRepository
        {
            get
            {
                return _deviceRepository ??= new DevicesRepository(_context);
            }
        }
        public IScheduleRepository scheduleRepository
        {
            get
            {
                return _scheduleRepository ??= new ScheduleRepository(_context);
            }
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

        public IOrderRepository orderRepository
        {
            get
            {
                return _orderRepository ??= new OrderRepository(_context);
            }
        }

        public IOrderDetailRepository orderDetailRepository
        {
            get
            {
                return _orderDetailRepository ??= new OrderDetailRepository(_context);
            }
        }

        public IPaymentRepository paymentRepository
        {
            get
            {
                return _paymentRepository ??= new PaymentRepository(_context);
            }
        }

        public IInventoryRepository inventoryRepository
        {
            get
            {
                return _inventoryRepository ??= new InventoryRepository(_context);
            }
        }

 
        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
