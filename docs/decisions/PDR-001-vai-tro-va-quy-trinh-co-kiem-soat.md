# PDR-001: Ba vai trò tài khoản và quy trình khóa luận có kiểm soát

## Trạng thái

Đã chấp nhận

## Ngày

2026-10-23

## Bối cảnh

Quy trình khóa luận trong báo cáo hiện bị phân tán qua email và bảng tính. Cần xác định rõ ai vận hành từng bước và tránh tạo thêm loại tài khoản không được nguồn nghiệp vụ nêu.

## Quyết định

Hệ thống có ba vai trò tài khoản: **Sinh viên**, **Giảng viên** và **Giáo vụ**. Giáo vụ thực hiện thao tác thay mặt Khoa; không có tài khoản Trưởng khoa riêng. Vai trò hướng dẫn, phản biện, chủ tịch và thư ký là trách nhiệm theo đề tài/hội đồng của tài khoản Giảng viên, không phải vai trò tài khoản mới.

Quy trình được quản lý xuyên suốt theo đợt: đăng ký/đề xuất đề tài, thực hiện và nộp tài liệu, phân công/bảo vệ, chấm điểm, xác nhận và công bố. Các trạng thái, quy tắc và điểm chưa chốt được duy trì trong tài liệu yêu cầu.

## Hệ quả

- Phân quyền phải xét cả vai trò tài khoản lẫn phân công theo đề tài/hội đồng.
- Dữ liệu và điểm của Sinh viên phải tách riêng trong cùng một nhóm.
- Không được thêm Admin, Trưởng khoa hay các vai trò nghiệp vụ mới chỉ vì tiện hiện thực nếu chưa có quyết định mới.
- Các ngoại lệ chưa có quy tắc, ví dụ thay đổi hội đồng đã có điểm, phải đi qua `OPEN_QUESTIONS.md` trước khi code.
