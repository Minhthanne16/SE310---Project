using Backend.Domain.Interfaces;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Sử dụng In-Memory Database cho môi trường phát triển & chạy nhanh out-of-the-box
        // Dễ dàng thay đổi thành SqlServer, Sqlite, hoặc PostgreSQL khi cần
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseInMemoryDatabase("SE310Database");
        });

        services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }
}
