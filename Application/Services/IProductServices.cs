using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.ViewModel.Request.ProductRequestDTO;
using static Application.ViewModel.Response.ProductResponse;

namespace Application.Services
{
    public interface IProductServices
    {
        Task<ResponseDTO> GetAllProductAsync(int pageIndex, int pageSize);
        Task<ResponseDTO> GetProductByIdAsync(long productId);
        Task<ResponseDTO> GetProductByNameAsync(string productName, int pageIndex, int pageSize);
        //Task<ResponseDTO> CreateProductAsync(CreateProductDTO request);
        Task<ResponseDTO> UpdateProductById(long productId, UpdateProductDTO request);
        Task<ResponseDTO> ChangeProductStatusById(long productId);
        Task<ResponseDTO> ChangeProductQuantityById(long productId, UpdateQuantityDTO request);
        Task<ResponseDTO> GetAllProductWithFilterAsync(int pageIndex, int pageSize, Status? status = null, long? categoryId = null,
                                                                    bool sortByStockAsc = true);
       // Task<ResponseDTO> DeleteProductByIdAsync(long productId);
    }
}
