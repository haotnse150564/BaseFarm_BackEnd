using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.ViewModel.Request.ProductRequest;
using static Application.ViewModel.Response.ProductResponse;

namespace Application.Services
{
    public interface IProductServices
    {
        Task<ResponseDTO> GetAllProductAsync(int pageIndex, int pageSize);
        Task<ResponseDTO> GetProductByIdAsync(long productId);
        Task<ResponseDTO> GetProductByNameAsync(string productName, int pageIndex, int pageSize);
        Task<ResponseDTO> CreateProductAsync(CreateProductDTO request);
        Task<ResponseDTO> UpdateProductById(long productId, CreateProductDTO request);
        Task<ResponseDTO> ChangeProductStatusById(long productId);
    }
}
