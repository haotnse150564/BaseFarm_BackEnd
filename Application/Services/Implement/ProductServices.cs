using Application.Commons;
using Application.Interfaces;
using Application.Utils;
using AutoMapper;
using Domain.Enum;
using Domain.Model;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using static Application.ViewModel.Request.ProductRequestDTO;
using static Application.ViewModel.Response.ProductResponse;

namespace Application.Services.Implement
{
    class ProductServices : IProductServices
    {
        private readonly IUnitOfWorks _unitOfWork;
        private readonly ICurrentTime _currentTime;
        private readonly IConfiguration configuration;
        private readonly IMapper _mapper;
        private readonly JWTUtils _jwt;

        public ProductServices(IUnitOfWorks unitOfWork, ICurrentTime currentTime, IConfiguration configuration, IMapper mapper, JWTUtils jwt)
        {
            _unitOfWork = unitOfWork;
            _currentTime = currentTime;
            this.configuration = configuration;
            _mapper = mapper;
            _jwt = jwt;
        }

        public async Task<ResponseDTO> GetAllProductAsync(int pageIndex, int pageSize)
        {
            try
            {
                var list = await _unitOfWork.productRepository.GetAllAsync();
                var products = list.Where(x => x.Status == ProductStatus.ACTIVE).ToList();  
                // Map dữ liệu sang DTO
                var result = _mapper.Map<List<ViewProductDTO>>(products);

                // Tạo đối tượng phân trang
                var pagination = new Pagination<ViewProductDTO>
                {
                    TotalItemCount = products.Count(),
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


        //public async Task<ResponseDTO> CreateProductAsync(CreateProductDTO request)
        //{
        //    try
        //    {
        //        var category = await _unitOfWork.categoryRepository.GetAllAsync();

        //        if (await _unitOfWork.productRepository.ExistsByNameAsync(request.ProductName))
        //        {
        //            return new ResponseDTO(Const.FAIL_CREATE_CODE, "The Product Name already exists. Please choose a different Product Name.");
        //        }
        //        else if (!category.Exists(x => x.CategoryId == request.CategoryId))
        //        {
        //            return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, "Category not exist!");
        //        }
        //        // Ánh xạ từ DTO sang Entity
        //        var product = _mapper.Map<Product>(request);
        //        product.Status = ProductStatus.ACTIVE;
        //        product.CreatedAt = DateOnly.FromDateTime(DateTime.Now);
        //        product.StockQuantity = 0;
        //        // Gọi AddAsync nhưng không gán vào biến vì nó không có giá trị trả về
        //        await _unitOfWork.productRepository.AddAsync(product);
        //        if (await _unitOfWork.SaveChangesAsync() < 0) ;
        //        {
        //            var crop = await _unitOfWork.cropRepository.GetByIdAsync(request.CropId);
        //            if (crop == null)
        //            {
        //                return new ResponseDTO(Const.FAIL_CREATE_CODE, "Crop not exists.");
        //            }
        //            crop.Status = CropStatus.IN_STOCK;
        //            await _unitOfWork.cropRepository.UpdateAsync(crop);
        //        }
        //        var check = await _unitOfWork.SaveChangesAsync();
        //        // Kiểm tra xem sản phẩm có được thêm không bằng cách kiểm tra product.Id (hoặc khóa chính)
        //        if (check < 0) // Nếu Id chưa được gán, có thể việc thêm đã thất bại
        //        {
        //            return new ResponseDTO(Const.FAIL_CREATE_CODE, "Failed to add product.");
        //        }

        //        return new ResponseDTO(Const.SUCCESS_CREATE_CODE, "Product registered successfully");
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
        //    }
        //}

        public async Task<ResponseDTO> UpdateProductById(long productId, UpdateProductDTO request)
        {
            try
            {
                var product = await _unitOfWork.productRepository.GetProductById(productId);
                var category = await _unitOfWork.categoryRepository.GetAllAsync();
                var updatedProduct = new Product();
                if (product == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, "Product not found !");
                }
                else if (!category.Exists(x => x.CategoryId == request.CategoryId))
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, "Category not exist!");
                }
                #region crop-cropother

                //else if (product.ProductNavigation.CropId != request.CropId)
                //{
                //    var products = product;

                //    //Đổi status và đổi product sang crop nếu cropId có thay đổi
                //    var crop = await _unitOfWork.cropRepository.GetByIdAsync(product.ProductNavigation.CropId);
                //    crop.Status = Status.ACTIVE;
                //    crop.Product = null;
                //    await _unitOfWork.cropRepository.UpdateAsync(crop);

                //    var cropUpdate = await _unitOfWork.cropRepository.GetByIdAsync(request.CropId);
                //    cropUpdate.Product = products;
                //    cropUpdate.Status = Status.DEACTIVATED;
                //    await _unitOfWork.cropRepository.UpdateAsync(cropUpdate);
                //    // ------------------------------------------------------------
                //    #region product update
                //    updatedProduct.ProductId = product.ProductId;
                //    updatedProduct.ProductName = request.ProductName;
                //    updatedProduct.CategoryId = (long)request.CategoryId;
                //    updatedProduct.StockQuantity = request.StockQuantity;
                //    updatedProduct.Images = request.Images;
                //    updatedProduct.Price = request.Price;
                //    updatedProduct.Description = request.Description;
                //    updatedProduct.UpdatedAt = DateOnly.FromDateTime(DateTime.Now);
                //    updatedProduct.Status = product.Status;
                //    updatedProduct.CreatedAt = product.CreatedAt;
                //    #endregion

                //    // Lưu các thay đổi vào cơ sở dữ liệu
                //    await _unitOfWork.productRepository.UpdateAsync(updatedProduct);
                //    var checkSave = await _unitOfWork.SaveChangesAsync();
                //    if(checkSave < 0)
                //    {
                //        return new ResponseDTO(Const.FAIL_CREATE_CODE, "Failed to update product.");
                //    }
                //    var result = _mapper.Map<ProductDetailDTO>(updatedProduct);
                //    result.CropName = (await _unitOfWork.cropRepository.GetByIdAsync(request.CropId)).CropName;
                //    result.CropId = request.CropId.ToString();
                //    result.CategoryName = category.Where(x => x.CategoryId == result.CategoryId).FirstOrDefault().CategoryName;

                //    return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_UPDATE_MSG, result);
                //}
                #endregion

                else
                {
                    // Sử dụng AutoMapper để ánh xạ thông tin từ DTO
                    #region product update
                    updatedProduct.ProductId = product.ProductId;
                    updatedProduct.ProductName = request.ProductName;
                    updatedProduct.CategoryId = (long)request.CategoryId;
                    updatedProduct.StockQuantity = 0;
                    updatedProduct.Images = request.Images;
                    updatedProduct.Price = request.Price;
                    updatedProduct.Description = request.Description;
                    updatedProduct.UpdatedAt = DateOnly.FromDateTime(DateTime.Now);
                    updatedProduct.Status = product.Status;
                    updatedProduct.CreatedAt = product.CreatedAt;
                    #endregion


                    // Lưu các thay đổi vào cơ sở dữ liệu
                    await _unitOfWork.productRepository.UpdateAsync(updatedProduct);
                    var checkSave = await _unitOfWork.SaveChangesAsync();
                    if (checkSave < 0)
                    {
                        return new ResponseDTO(Const.FAIL_CREATE_CODE, "Failed to update product.");
                    }
                    var result = _mapper.Map<ProductDetailDTO>(updatedProduct);
                    //result.CropId = product.ProductNavigation.CropId.ToString();
                    result.CropName = product.ProductNavigation.CropName;
                    //result.CategoryName = category.Where(x => x.CategoryId == result.).FirstOrDefault().CategoryName;
                    result.StockQuantity = product.StockQuantity;

                    return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_UPDATE_MSG, result);
                }
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

                product.Status = (product.Status == ProductStatus.ACTIVE) ? ProductStatus.DEACTIVED : ProductStatus.ACTIVE;

                // Lưu các thay đổi vào cơ sở dữ liệu
                await _unitOfWork.productRepository.UpdateAsync(product);
                await _unitOfWork.SaveChangesAsync();
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

        public Task<ResponseDTO> CreateProductAsync(CreateProductDTO request)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDTO> DeleteProductByIdAsync(long productId)
        {
            var user = await _jwt.GetCurrentUserAsync();
            if (user == null || user.Role != Roles.Manager)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, "Tài khoản không hợp lệ.");
            }
            try
            {
                var product = await _unitOfWork.productRepository.GetProductById(productId);
                if (product == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, "Product not found.");
                }
                // Xóa sản phẩm
                await _unitOfWork.productRepository.DeleteAsync(product);
                await _unitOfWork.SaveChangesAsync();
                var crop = await _unitOfWork.cropRepository.GetByIdAsync(productId);

                if (crop == null)
                {
                    return new ResponseDTO(Const.SUCCESS_READ_CODE, "Product deleted successfully.");
                }
                else { 
                await _unitOfWork.cropRepository.DeleteAsync(crop);
                await _unitOfWork.SaveChangesAsync();
                }
                return new ResponseDTO(Const.SUCCESS_READ_CODE, "Product deleted successfully.");
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.ERROR_EXCEPTION, ex.Message);
            }
        }
    }
}
