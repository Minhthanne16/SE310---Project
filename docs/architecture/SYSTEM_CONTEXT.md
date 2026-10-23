# Bối cảnh hệ thống

QLKLTN là ranh giới trung tâm cho thông tin và thao tác nghiệp vụ của một đợt khóa luận trong một Khoa.

## Tác nhân và tương tác

| Tác nhân | Tương tác với hệ thống |
| --- | --- |
| Sinh viên | Đăng ký/đề xuất đề tài, nộp tài liệu, nhận phản hồi, xem lịch và kết quả cá nhân. |
| Giảng viên | Quản lý đề tài hướng dẫn, xét đề xuất, nhận xét tài liệu, thực hiện phản biện/hội đồng và chấm điểm theo phân công. |
| Giáo vụ | Vận hành đợt, quản lý dữ liệu nền, phân công, thiết lập bảo vệ, xác nhận/công bố kết quả. |
| Khoa | Không phải tài khoản riêng; các thao tác nghiệp vụ của Khoa được Giáo vụ thực hiện theo báo cáo. |

## Ranh giới

- Bên trong hệ thống: dữ liệu đề tài, nhóm, tài liệu, phân công, lịch, phiếu điểm, thông báo và lịch sử xử lý.
- Bên ngoài hệ thống: quyết định sinh viên có đủ điều kiện làm khóa luận; quy định chi tiết của Khoa; trao đổi chuyên môn không được đặc tả là thông báo/nhận xét trên hệ thống.
- Hệ thống cần dịch vụ lưu trữ dữ liệu và kết nối Internet. Không có tích hợp bên thứ ba nào được báo cáo xác nhận để hiện thực ở giai đoạn này.

## Hướng kỹ thuật hiện tại

Repo hiện có frontend Next.js và backend ASP.NET Core theo `README.md`, nhưng đây là nền kỹ thuật có sẵn chứ chưa xác nhận là hiện thực QLKLTN. Trước mọi thay đổi API hoặc database, phải đối chiếu mã nguồn thực tế với toàn bộ tài liệu nghiệp vụ và các câu hỏi mở.
