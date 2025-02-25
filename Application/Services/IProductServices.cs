﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.ViewModel.Response.ProductResponse;

namespace Application.Services
{
    public interface IProductServices
    {
        Task<ResponseDTO> GetAllProductAsync(int pageIndex, int pageSize);
        Task<ResponseDTO> GetProductByIdAsync(int productId);
        Task<ResponseDTO> GetProductByNameAsync(string productName);
    }
}
