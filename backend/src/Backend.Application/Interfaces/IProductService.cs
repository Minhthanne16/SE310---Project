using Backend.Application.Common;
using Backend.Application.DTOs.Product;

namespace Backend.Application.Interfaces;

public interface IProductService
{
    Task<ApiResponse<IReadOnlyList<ProductDto>>> GetAllProductsAsync();
    Task<ApiResponse<ProductDto>> GetProductByIdAsync(Guid id);
    Task<ApiResponse<ProductDto>> CreateProductAsync(CreateProductDto dto);
    Task<ApiResponse<ProductDto>> UpdateProductAsync(Guid id, UpdateProductDto dto);
    Task<ApiResponse<bool>> DeleteProductAsync(Guid id);
}
