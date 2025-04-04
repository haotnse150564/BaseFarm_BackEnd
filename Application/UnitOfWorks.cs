﻿using Application;
using Application.Interfaces;
using Application.Services;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Implement;
using Microsoft.EntityFrameworkCore;
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
        private IIoTdeviceRepository _ioTdeviceRepository;

        public UnitOfWorks(AppDbContext context, ICurrentTime currentTime, IClaimsServices claimsServices, IProductRepository productRepository
            , IFeedbackRepository feedbackRepository, IAccountProfileRepository accountProfileRepository, IOrderRepository orderRepository
            , IOrderDetailRepository orderDetailRepository, IPaymentRepository paymentRepository, IScheduleRepository scheduleRepository
            , IIoTdeviceRepository ioTdeviceRepository)
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
            _ioTdeviceRepository = ioTdeviceRepository;
        }
        public IIoTdeviceRepository ioTdeviceRepository
        {
            get
            {
                return _ioTdeviceRepository ??= new IoTdeviceRepository(_context);
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
