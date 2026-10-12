using Backend.Application.Common;
using Backend.Application.DTOs.Product;
using Backend.Application.Interfaces;
using Backend.Domain.Entities;
using Backend.Domain.Interfaces;

namespace Backend.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ApiResponse<IReadOnlyList<ProductDto>>> GetAllProductsAsync()
    {
        var products = await _repository.GetAllAsync();
        var dtos = products.Select(MapToDto).ToList();
        return ApiResponse<IReadOnlyList<ProductDto>>.Ok(dtos, "Lấy danh sách sản phẩm thành công");
    }

    public async Task<ApiResponse<ProductDto>> GetProductByIdAsync(Guid id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
        {
            return ApiResponse<ProductDto>.Fail("Không tìm thấy sản phẩm với Id đã cung cấp.");
        }

        return ApiResponse<ProductDto>.Ok(MapToDto(product), "Lấy thông tin sản phẩm thành công");
    }

    public async Task<ApiResponse<ProductDto>> CreateProductAsync(CreateProductDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return ApiResponse<ProductDto>.Fail("Tên sản phẩm không được để trống.");
        }

        if (dto.Price < 0)
        {
            return ApiResponse<ProductDto>.Fail("Giá sản phẩm phải lớn hơn hoặc bằng 0.");
        }

        var product = new Product
        {
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim() ?? string.Empty,
            Price = dto.Price,
            Stock = dto.Stock,
            IsActive = true
        };

        var created = await _repository.AddAsync(product);
        return ApiResponse<ProductDto>.Ok(MapToDto(created), "Tạo sản phẩm mới thành công");
    }

    public async Task<ApiResponse<ProductDto>> UpdateProductAsync(Guid id, UpdateProductDto dto)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
        {
            return ApiResponse<ProductDto>.Fail("Không tìm thấy sản phẩm cần cập nhật.");
        }

        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return ApiResponse<ProductDto>.Fail("Tên sản phẩm không được để trống.");
        }

        product.Name = dto.Name.Trim();
        product.Description = dto.Description?.Trim() ?? string.Empty;
        product.Price = dto.Price;
        product.Stock = dto.Stock;
        product.IsActive = dto.IsActive;
        product.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(product);
        return ApiResponse<ProductDto>.Ok(MapToDto(product), "Cập nhật sản phẩm thành công");
    }

    public async Task<ApiResponse<bool>> DeleteProductAsync(Guid id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
        {
            return ApiResponse<bool>.Fail("Không tìm thấy sản phẩm để xóa.");
        }

        await _repository.DeleteAsync(id);
        return ApiResponse<bool>.Ok(true, "Xóa sản phẩm thành công");
    }

    private static ProductDto MapToDto(Product entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Description = entity.Description,
        Price = entity.Price,
        Stock = entity.Stock,
        IsActive = entity.IsActive,
        CreatedAt = entity.CreatedAt
    };
}
