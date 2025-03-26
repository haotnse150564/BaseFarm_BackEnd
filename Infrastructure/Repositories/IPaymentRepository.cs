﻿using Application.Repositories;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        Task<Payment> GetByTransactionIdAsync(string transactionId);
        Task<Payment> GetByOrderIdAsync(long orderId);
    }
}
