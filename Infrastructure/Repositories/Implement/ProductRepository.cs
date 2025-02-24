using Application.Interfaces;
using Application.Services;
using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implement
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context, ICurrentTime timeService, IClaimsServices claimsService)
            : base(context, timeService, claimsService) => _context = context;

        public async Task<List<Product?>> getAllProductAsync()
        {
            return await _context.Product.ToListAsync();
        }
    }
}
