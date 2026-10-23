# Ràng buộc phi chức năng

Chỉ các ràng buộc có căn cứ từ báo cáo được ghi là yêu cầu hiện tại. Không có chỉ số hiệu năng, dung lượng hay SLA nào được tự đặt ra.

| Nhóm | Ràng buộc đã biết |
| --- | --- |
| Nền tảng | Hệ thống là ứng dụng web, truy cập qua trình duyệt và cần kết nối Internet. |
| Dữ liệu | Dữ liệu khóa luận, tài liệu, phân công và kết quả được lưu tập trung; cần giữ lịch sử thay đổi/tài liệu để tra cứu. |
| Phân quyền | Người dùng có tài khoản và quyền phù hợp vai trò; kiểm tra quyền phải ở backend. |
| Tính đúng đắn | Các thay đổi liên quan đăng ký, phân công và kết quả phải giữ nhất quán, đặc biệt khi có thao tác đồng thời. |
| Vận hành | Hệ thống phụ thuộc vào máy chủ, cơ sở dữ liệu và kết nối Internet. Các mốc/thời hạn chịu sự điều chỉnh của Khoa. |

Trước khi triển khai các phần liên quan upload, bảo mật, tải cao hoặc khôi phục dữ liệu, phải chốt các mục OQ-006 và OQ-007 trong [OPEN_QUESTIONS.md](../OPEN_QUESTIONS.md).
