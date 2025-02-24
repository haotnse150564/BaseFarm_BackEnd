using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.ViewModel.Response.Product;

namespace Application.Services
{
    public interface IProductServices
    {
        Task<ResponseDTO> GetAllProductAsync();
    }
}
