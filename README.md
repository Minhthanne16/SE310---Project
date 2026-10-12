# SE310 Project - React (TypeScript) & ASP.NET Core 9 Web API

Repository mẫu hoàn chỉnh gồm Frontend (React + TypeScript) và Backend (.NET 9 C#) được phân tách thành 2 thư mục riêng biệt, tuân thủ các chuẩn kiến trúc phần mềm hiện đại và thực tế trong doanh nghiệp.

---

## 🏛️ Kiến Trúc Được Áp Dụng

### 1. Backend: Clean Architecture (Onion / Hexagonal Architecture)
Dự án Backend được chia thành 4 tầng theo nguyên lý **Dependency Inversion** (mũi tên phụ thuộc hướng vào trung tâm):

```
       ┌────────────────────────┐
       │   Presentation (API)   │  Controllers, Middlewares, Program.cs
       └───────────┬────────────┘
                   │
       ┌───────────▼────────────┐
       │      Application       │  Use Cases, Services, DTOs, Business Logic
       └───────────┬────────────┘
                   │
       ┌───────────▼────────────┐
       │         Domain         │  Entities, Interfaces, Enums (Không phụ thuộc tầng khác)
       └────────────────────────┘
                   ▲
       ┌───────────┴────────────┐
       │     Infrastructure     │  EF Core DbContext, Repositories, Database Providers
       └────────────────────────┘
```

- **Domain (`Backend.Domain`)**: Trung tâm cốt lõi của nghiệp vụ, chứa các Entity (`Product`, `BaseEntity`) và Interface nghiệp vụ (`IProductRepository`). Không phụ thuộc vào bất kỳ thư viện bên ngoài hay cơ sở dữ liệu nào.
- **Application (`Backend.Application`)**: Chứa logic xử lý nghiệp vụ (`ProductService`), DTOs (`ProductDto`, `CreateProductDto`), chuẩn hóa phản hồi (`ApiResponse<T>`).
- **Infrastructure (`Backend.Infrastructure`)**: Triển khai chi tiết tầng kỹ thuật, cấu hình Entity Framework Core (`ApplicationDbContext`), Seed dữ liệu (`DataSeeder`), và Repositories (`ProductRepository`). Sử dụng In-Memory DB (sẵn sàng chuyển đổi sang SQL Server/PostgreSQL).
- **API (`Backend.API`)**: Điểm vào của ứng dụng (RESTful Controllers), xử lý HTTP requests/responses, Global Exception Handling Middleware, CORS và Swagger OpenApi documentation.

---

### 2. Frontend: Feature-based Architecture (Kiến trúc theo tính năng)
Frontend sử dụng Vite + React 19 + TypeScript, cấu trúc theo từng module tính năng (Features):

```
frontend/src/
├── api/                  # Axios HTTP client cấu hình tập trung & Interceptors
├── components/common/    # Component giao diện dùng chung (Header, Modal, Buttons)
├── features/             # Chia thư mục theo từng nghiệp vụ/tính năng
│   └── products/
│       ├── components/   # ProductCard, ProductFormModal
│       └── services/     # Product API Service
├── types/                # TypeScript Interfaces, Models, API Responses
├── App.tsx               # Component chính điều phối view
├── main.tsx              # Entry point của ứng dụng
└── index.css             # Design tokens, CSS variables & utilities
```

---

## 📂 Cấu Trúc Thư Mục Repository

```text
SE310-Project/
├── backend/
│   ├── Backend.sln
│   └── src/
│       ├── Backend.Domain/
│       ├── Backend.Application/
│       ├── Backend.Infrastructure/
│       └── Backend.API/
├── frontend/
│   ├── src/
│   ├── index.html
│   ├── package.json
│   ├── tsconfig.json
│   └── vite.config.ts
├── .gitignore
└── README.md
```

---

## 🚀 Hướng Dẫn Cài Đặt & Chạy Ứng Dụng

### 1. Khởi chạy Backend (.NET 9)
```bash
cd backend
dotnet restore
dotnet run --project src/Backend.API
```
- API Base URL: `http://localhost:5290`
- Swagger UI (tài liệu API trực quan): `http://localhost:5290/swagger`

### 2. Khởi chạy Frontend (React + TypeScript)
Mở một cửa sổ Terminal mới:
```bash
cd frontend
npm install
npm run dev
```
- Frontend Web App: `http://localhost:5173`

---

## 🔌 API Endpoints Mẫu (Products)

| Method | Endpoint | Mô tả |
| :--- | :--- | :--- |
| `GET` | `/api/products` | Lấy danh sách sản phẩm |
| `GET` | `/api/products/{id}` | Lấy chi tiết sản phẩm theo ID |
| `POST` | `/api/products` | Thêm mới sản phẩm |
| `PUT` | `/api/products/{id}` | Cập nhật thông tin sản phẩm |
| `DELETE` | `/api/products/{id}` | Xóa sản phẩm theo ID |
