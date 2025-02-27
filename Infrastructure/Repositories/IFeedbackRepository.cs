using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IFeedbackRepository
    {
        Task<int> CountAsync();
        Task<IEnumerable<Feedback>> GetPagedAsync(int pageIndex, int pageSize);
    }
}
