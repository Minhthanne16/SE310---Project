# Hướng dẫn cho agent làm việc trong SE310-Project

## Phạm vi và cấu trúc

- File này áp dụng cho toàn bộ repository. Đọc `README.md` và file liên quan trước khi sửa.
- `frontend/`: Next.js App Router, React, TypeScript. Client API ở `frontend/src/api/request.ts` và `frontend/src/features/*/services/`; kiểu dữ liệu ở `frontend/src/types/`. `frontend/next.config.ts` chuyển `/api/*` sang backend.
- `backend/src/Backend.API/`: controller, middleware, cấu hình HTTP. `backend/src/Backend.Application/`: DTO, command/query, validation, response. `backend/src/Backend.Domain/`: entity và interface. `backend/src/Backend.Infrastructure/`: EF Core, migration, repository, tích hợp ngoài.
- `backend/tests/Backend.Tests/` và các file `*.test.ts(x)` là nơi kiểm tra hành vi.
- PostgreSQL local được cấu hình trong `compose.yaml`. Cách dùng Neon nằm ở [docs/NEON.md](docs/NEON.md).

## Skills cần chọn theo công việc

Chỉ dùng skill thực sự phù hợp với tác vụ, theo hướng dẫn của môi trường agent:

| Công việc | Skill |
| --- | --- |
| Sửa lỗi hoặc gặp test lỗi, hành vi bất thường | `systematic-debugging` |
| Thay đổi logic/hành vi, thêm endpoint, sửa bug | `test-driven-development` |
| Nhận input, xác thực, lưu dữ liệu, tích hợp dịch vụ ngoài | `security-and-hardening` |
| Làm giao diện/trang/component mới | `frontend-design` |
| Refactor để dễ đọc mà giữ nguyên hành vi | `code-simplification` |
| Yêu cầu lớn cần chia thành các bước thực hiện | `planning-and-task-breakdown` |
| Thiết lập project Neon hoặc mở rộng backend trên Neon | `neon` |
| Kết nối PostgreSQL, schema, migration hoặc branch Neon | `neon-postgres` |

## Quy tắc bắt buộc khi sửa API

1. **Trước khi code backend**, kiểm tra frontend đang gọi endpoint nào và dùng hợp đồng nào: `frontend/src/api/request.ts`, `frontend/src/features/*/services/`, `frontend/src/types/`, các component gọi service, `frontend/next.config.ts` và test liên quan. Ghi lại method, URL, request body, response body, mã trạng thái, lỗi và trường hợp thiếu dữ liệu.
2. **Trước khi code frontend**, kiểm tra backend thực sự cung cấp gì: controller trong `backend/src/Backend.API/`, DTO/validator/handler trong `backend/src/Backend.Application/`, `ApiResponse<T>`, middleware lỗi và OpenAPI/Scalar khi API đang chạy. Không suy đoán endpoint từ tên file hoặc chỉ dựa vào type phía frontend.
3. Mỗi thay đổi hợp đồng API phải đối chiếu **cả hai phía trước khi sửa**, cập nhật cả hai phía trong cùng tác vụ nếu cần, rồi kiểm tra lại bằng test hoặc request thực tế. Không âm thầm đổi tên field, kiểu, nullability, route, status code, cấu trúc lỗi hay quy tắc xác thực.
4. Quy ước hiện tại: frontend gọi `/api/products`; Next.js proxy đến backend; response thành công dùng `{ success, message, data, errors? }`. `apiRequest<T>` trả `data` và ném lỗi nếu HTTP lỗi hoặc `success=false`. Backend dùng `api/[controller]` với `ProductsController` cho GET/POST/PUT/DELETE. Kiểm tra mã nguồn hiện tại vì quy ước có thể thay đổi.
5. Khi thay đổi entity hoặc DTO có ảnh hưởng database, kiểm tra mapping EF Core và migration; không sửa migration đã áp dụng chỉ để che sai lệch. Xem kỹ SQL/migration trước khi áp dụng lên dữ liệu có thật.

## Cách làm và kiểm tra

- Giữ ranh giới các tầng hiện có. Đặt logic nghiệp vụ ở Application, truy cập dữ liệu ở Infrastructure, HTTP ở API; frontend không kết nối trực tiếp PostgreSQL/Neon.
- Không commit mật khẩu, connection string thật, token hoặc file dump. Dùng biến môi trường/secrets; không in secret ra log hoặc kết quả kiểm tra.
- Không tự chạy migration, seed, restore hoặc xóa trên database dùng chung/production chỉ để kiểm tra code. Xác định đúng database/branch trước khi thao tác.
- Chỉ chạy kiểm tra phù hợp với phần đã sửa. Backend: từ `backend/` chạy `dotnet restore Backend.sln --locked-mode`, `dotnet test Backend.sln --no-restore`; frontend: từ `frontend/` chạy `npm ci`, `npm run typecheck`, `npm run lint`, `npm test`, `npm run build` khi tương ứng. Nếu không chạy được, nêu rõ lý do.
- Tôn trọng thay đổi có sẵn trong working tree; không hoàn tác hoặc ghi đè thay đổi ngoài phạm vi tác vụ.
