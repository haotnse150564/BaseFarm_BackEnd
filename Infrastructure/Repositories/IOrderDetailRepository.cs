using Application.Repositories;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IOrderDetailRepository : IGenericRepository<OrderDetail>
    {
    }
}
