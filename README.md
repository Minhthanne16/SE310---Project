# SE310 Project

Ứng dụng quản lý sản phẩm mẫu với Next.js 16 và ASP.NET Core 10. Frontend và backend nằm trong hai thư mục riêng, kết nối qua `/api` của Next.js.

## Tech stack

| Phần | Công nghệ |
| --- | --- |
| Frontend | Node.js 24, Next.js 16.4.0 App Router, React 19.3.0, TypeScript 5.9.3 strict mode |
| UI | Tailwind CSS 4.3.3, React Hot Toast, Lucide React |
| FE testing/tooling | Vitest 5.0.3, Testing Library, jsdom, Oxlint 1.87.0, Prettier 3.9.9, Turbopack |
| Backend | ASP.NET Core Web API, .NET 10, Clean Architecture, CQRS/MediatR 14.2.0, Repository |
| Validation/mapping | FluentValidation 12.1.1, AutoMapper 16.2.0 |
| Data | PostgreSQL hoặc Neon, EF Core 10.0.12, Npgsql 10.0.3 |
| Integrations | Redis 8/StackExchange.Redis, MinIO, JWT Bearer, BCrypt, Google Identity SDK, SMTP/MailKit |
| API/tests | OpenAPI + Scalar, xUnit, ASP.NET Core TestHost, coverlet |
| Delivery | Docker multi-stage, Docker Compose, GHCR workflow, GitHub Actions, npm audit, Gitleaks |

## Cấu trúc

```text
frontend/
  app/                         Next.js App Router
  src/features/products/       Giao diện và client cho sản phẩm
  src/api/                     HTTP client qua Next.js proxy
  Dockerfile
backend/
  src/Backend.Domain/         Entity và repository contract
  src/Backend.Application/    CQRS handlers, validation, mapping
  src/Backend.Infrastructure/ EF Core, PostgreSQL migration, Redis/MinIO/SMTP adapters
  src/Backend.API/            HTTP controllers, OpenAPI/Scalar, JWT config
  tests/Backend.Tests/        xUnit và TestHost
  Dockerfile
compose.yaml
.github/workflows/             CI và publish GHCR theo lệnh thủ công
```

CRUD sản phẩm chạy qua MediatR và EF Core PostgreSQL. Tích hợp Redis, MinIO, SMTP, JWT/Google và BCrypt đã có package, cấu hình và adapter cơ bản; repo **chưa có** endpoint đăng nhập, refresh-token rotation, gửi email hoặc upload file. Các luồng đó cần được xây khi có yêu cầu nghiệp vụ cụ thể.

## Chạy bằng Docker Compose

1. Sao chép `.env.example` thành `.env` rồi thay các giá trị mật khẩu mẫu. `.env` được git bỏ qua.
2. Chạy `docker compose up --build` tại thư mục gốc.
3. Mở frontend tại <http://localhost:3000>, API tại <http://localhost:5290>, Scalar tại <http://localhost:5290/scalar> và MinIO Console tại <http://localhost:9001>.

Compose tự chạy migration và seed ba sản phẩm mẫu. Volume PostgreSQL, Redis và MinIO giữ dữ liệu giữa các lần khởi động.

### Dùng Neon thay PostgreSQL local

Project Neon `se310-project` đã có migration khởi tạo. Đặt chuỗi kết nối pooled vào `ConnectionStrings__DefaultConnection` trong `.env` local (xem `.env.example`), rồi chạy `docker compose -f compose.neon.yaml up --build`. Cấu hình này không tự chạy migration hoặc seed. Xem [hướng dẫn Neon](docs/NEON.md) để lấy lại chuỗi kết nối, kiểm tra branch và chuyển dữ liệu nếu cần.

MinIO trong Compose chỉ dành cho local development. Bản open-source này đã được lưu trữ và có [cảnh báo bảo mật từ dự án MinIO](https://github.com/minio/minio/security/advisories); các cổng trong Compose chỉ bind vào `127.0.0.1`. Khi triển khai thực tế, dùng dịch vụ MinIO được duy trì và quản lý riêng.

## Chạy riêng để phát triển

Yêu cầu Node.js 24, .NET SDK 10 và PostgreSQL. Khai báo biến môi trường `ConnectionStrings__DefaultConnection` cho API, ví dụ chuỗi kết nối tới PostgreSQL local. Với Neon, dùng chuỗi kết nối do Neon cấp qua biến môi trường; không ghi vào file được commit.

```powershell
cd backend
dotnet restore Backend.sln --locked-mode
dotnet tool restore
dotnet tool run dotnet-ef database update --project src/Backend.Infrastructure --startup-project src/Backend.API
dotnet run --project src/Backend.API
```

Ở terminal khác:

```powershell
cd frontend
npm ci
npm run dev
```

`frontend/next.config.ts` chuyển `/api/*` đến `http://localhost:5290` khi chạy local. Có thể đặt `API_INTERNAL_URL` nếu API ở địa chỉ khác. Đặt `InitializeDatabase=true` để API tự chạy migration khi khởi động.

## Kiểm tra

```powershell
cd backend
dotnet restore Backend.sln --locked-mode
dotnet test Backend.sln --no-restore

cd ../frontend
npm run typecheck
npm run lint
npm test
npm run build
npm audit --audit-level=high
```

CI dùng `npm ci`, NuGet lockfile, build/test cả hai phần và Gitleaks. Workflow GHCR chỉ chạy khi được kích hoạt thủ công (`workflow_dispatch`).
