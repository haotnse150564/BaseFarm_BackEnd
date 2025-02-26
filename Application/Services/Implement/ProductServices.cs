using Application.Commons;
using Application.Interfaces;
using AutoMapper;
using Domain;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.ViewModel.Request.ProductRequest;
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

        public async Task<ResponseDTO> GetAllProductAsync(int pageIndex, int pageSize)
        {
            try
            {
                var listProduct = await _unitOfWork.productRepository.ToPaginationAsync(pageIndex, pageSize);

                if (listProduct == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "No Products found.");
                }

                // Map dữ liệu sang DTO
                var result = _mapper.Map<List<ViewProductDTO>>(listProduct);

                // Tạo đối tượng phân trang
                var pagination = new Pagination<ViewProductDTO>
                {
                    TotalItemCount = listProduct.TotalItemCount,
                    PageSize = pageSize,
                    PageIndex = pageIndex,
                    Items = result
                };

                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, pagination);
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

        public async Task<ResponseDTO> GetProductByNameAsync(string productName)
        {
            try
            {
                
                var product = await _unitOfWork.productRepository.GetProductByNameAsync(productName);

                // Kiểm tra nếu danh sách rỗng
                if (product == null || !product.Any())
                {
                    return new ResponseDTO(Const.SUCCESS_CREATE_CODE, "No Product found with the given name.");
                }

                // Sử dụng AutoMapper để ánh xạ các entity sang DTO
                var result = _mapper.Map<List<ViewProductDTO>>(product);

                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu xảy ra
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<ResponseDTO> CreateProductAsync(CreateProductDTO request)
        {
            try
            {
                if (await _unitOfWork.productRepository.ExistsByNameAsync(request.ProductName))
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, "The Product Name already exists. Please choose a different Product Name.");
                }

                // Ánh xạ từ DTO sang Entity
                var product = _mapper.Map<Product>(request);
                product.CreatedAt = DateOnly.FromDateTime(DateTime.Now);

                // Thêm sản phẩm vào DbSet
                 await _unitOfWork.productRepository.AddAsync(product);

                // Lưu thay đổi vào database
                var saveResult = await _unitOfWork.SaveChangesAsync();

                // Kiểm tra nếu lưu không thành công
                if (saveResult <= 0)
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, "Failed to register the product.");
                }

                return new ResponseDTO(Const.SUCCESS_CREATE_CODE, "Product registered successfully");
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

    }
}
