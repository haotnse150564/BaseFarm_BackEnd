using Domain.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implement
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        private readonly AppDbContext _context;
        public PaymentRepository(AppDbContext context) => _context = context;

        public async Task<Payment> GetByTransactionIdAsync(string transactionId)
        {
            return await _context.Payment
                .Include(u => u.Order)
                .FirstOrDefaultAsync(p => p.TransactionId == transactionId);
        }

        public async Task<Payment> GetByOrderIdAsync(long orderId)
        {
            var payment = await _context.Payment
                .Include(u => u.Order)
                .FirstOrDefaultAsync(p => p.OrderId == orderId);

            if (payment == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy Payment với OrderId = {orderId}");
            }

            return payment;

        }
    }
}
