﻿using Application.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.ViewModel.Response.ProductResponse;

namespace Application.Services.Implement
{
    class ProductServices : IProductServices
    {
        private readonly IUnitOfWorks _unitOfWork;
        private readonly ICurrentTime _currentTime;
        private readonly IConfiguration configuration;
        private readonly IMapper _mapper;

        public ProductServices(IUnitOfWorks unitOfWork, ICurrentTime currentTime, IConfiguration configuration, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentTime = currentTime;
            this.configuration = configuration;
            _mapper = mapper;
        }

        public async Task<ResponseDTO> GetAllProductAsync()
        {
            try
            {
                var listProduct = await _unitOfWork.productRepository.GetAllAsync();

                if (listProduct == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "No Products found.");
                }

                else
                {
                    var result = _mapper.Map<List<ViewProductDTO>>(listProduct);

                    return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
                }
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<ResponseDTO> GetProductByIdAsync(int productId)
        {
            try
            {
                // Gọi repository để lấy danh sách người dùng theo tên
                var productDetail = await _unitOfWork.productRepository.GetProductByCurrentId(productId);

                // Kiểm tra nếu danh sách rỗng
                if (productDetail == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "No Product found with the ID");
                }

                // Sử dụng AutoMapper để ánh xạ các entity sang DTO
                var result = _mapper.Map<ProductDetailDTO>(productDetail);

                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu xảy ra
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
    }
}
