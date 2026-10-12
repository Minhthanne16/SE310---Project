using Backend.API.Middlewares;
using Backend.Application;
using Backend.Infrastructure;
using Backend.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình CORS để Frontend (React/Vite) gọi API dễ dàng
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// 2. Đăng ký Services của các tầng Clean Architecture
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// 3. Đăng ký Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "SE310 API", Version = "v1", Description = "Backend API cho dự án môn SE310" });
});

var app = builder.Build();

// 4. Seed dữ liệu mẫu khi ứng dụng khởi chạy
await DataSeeder.SeedAsync(app.Services);

// 5. Middleware xử lý lỗi tập trung (Global Exception Handling)
app.UseMiddleware<GlobalExceptionMiddleware>();

// 6. Cấu hình HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SE310 API v1");
    });
}

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();

