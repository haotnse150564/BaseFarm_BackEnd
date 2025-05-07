using Application.Commons;
using Application.Interfaces;
using AutoMapper;
using Domain.Enum;
using Domain.Model;
using Microsoft.Extensions.Configuration;
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
                var result = _mapper.Map<List<ViewProductDTO>>(listProduct.Items);

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

        public async Task<ResponseDTO> GetAllProductWithFilterAsync(int pageIndex, int pageSize, Status? status = null, long? categoryId = null,
                                                                    bool sortByStockAsc = true)
        {
            try
            {
                var listProduct = await _unitOfWork.productRepository
                    .GetFilteredProductsAsync(pageIndex, pageSize, status, categoryId, sortByStockAsc);

                var result = _mapper.Map<List<ViewProductDTO>>(listProduct.Items);

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

        public async Task<ResponseDTO> GetProductByIdAsync(long productId)
        {
            try
            {
                // Gọi repository để lấy danh sách người dùng theo tên
                var productDetail = await _unitOfWork.productRepository.GetProductById(productId);

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

        public async Task<ResponseDTO> GetProductByNameAsync(string productName, int pageIndex, int pageSize)
        {
            try
            {
                // Đếm tổng số sản phẩm khớp với tên tìm kiếm
                var totalItemCount = await _unitOfWork.productRepository.CountByNameAsync(productName);

                // Lấy danh sách sản phẩm theo trang
                var listProduct = await _unitOfWork.productRepository
                                    .GetPagedByNameAsync(productName, pageIndex, pageSize);

                // Kiểm tra nếu danh sách rỗng
                if (listProduct == null || !listProduct.Any())
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "No Product found with the given name.");
                }

                // Ánh xạ dữ liệu sang DTO
                var result = _mapper.Map<List<ViewProductDTO>>(listProduct);

                // Tạo đối tượng phân trang
                var pagination = new Pagination<ViewProductDTO>
                {
                    TotalItemCount = totalItemCount,
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

                // Gọi AddAsync nhưng không gán vào biến vì nó không có giá trị trả về
                var added = _unitOfWork.productRepository.AddAsync(product);

                // Kiểm tra xem sản phẩm có được thêm không bằng cách kiểm tra product.Id (hoặc khóa chính)
                if (added==null) // Nếu Id chưa được gán, có thể việc thêm đã thất bại
                {
                    return new ResponseDTO(Const.FAIL_CREATE_CODE, "Failed to add product to repository.");
                }

                return new ResponseDTO(Const.SUCCESS_CREATE_CODE, "Product registered successfully");
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<ResponseDTO> UpdateProductById(long productId, CreateProductDTO request)
        {
            try
            {
                var product = await _unitOfWork.productRepository.GetProductById(productId);
                if (product == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, "Product not found !");
                }

                // Sử dụng AutoMapper để ánh xạ thông tin từ DTO
                var updatedProduct = _mapper.Map(request, product);

                var result = _mapper.Map<ProductDetailDTO>(updatedProduct);

                // Lưu các thay đổi vào cơ sở dữ liệu
                await _unitOfWork.productRepository.UpdateAsync(product);

                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_UPDATE_MSG, result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<ResponseDTO> ChangeProductStatusById(long productId)
        {
            try
            {
                var product = await _unitOfWork.productRepository.GetProductById(productId);
                if (product == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, "Product not found !");
                }

                product.Status = (product.Status == Status.ACTIVE) ? Status.ACTIVE : Status.SUSPENDED;

                // Lưu các thay đổi vào cơ sở dữ liệu
                await _unitOfWork.productRepository.UpdateAsync(product);

                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, "Change Status Succeed");
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }

        public async Task<ResponseDTO> ChangeProductQuantityById(long productId, UpdateQuantityDTO request)
        {
            try
            {
                var product = await _unitOfWork.productRepository.GetProductById(productId);
                if (product == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, "Product not found !");
                }

                // Sử dụng AutoMapper để ánh xạ thông tin từ DTO
                var updatedProduct = _mapper.Map(request, product);

                var result = _mapper.Map<ProductDetailDTO>(updatedProduct);

                // Lưu các thay đổi vào cơ sở dữ liệu
                await _unitOfWork.productRepository.UpdateAsync(product);

                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, "Change Quantity Succeed");
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
    }
}
