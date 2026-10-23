# Mô hình miền khái niệm

Đây là mô hình nghiệp vụ, không phải lược đồ bảng hay hợp đồng API.

| Khái niệm | Trách nhiệm | Liên hệ chính |
| --- | --- | --- |
| Người dùng | Đại diện Sinh viên, Giảng viên hoặc Giáo vụ | Tham gia nhóm, hướng dẫn/chấm, nhận thông báo, tạo lịch sử. |
| Đợt khóa luận | Bao bọc mốc và cấu hình cho một chu kỳ khóa luận | Có nhiều đề tài, hội đồng và mẫu phiếu được gán. |
| Đề tài | Trung tâm của vòng đời khóa luận | Thuộc một đợt, có hướng dẫn viên, nhóm, bài nộp, phân công và lịch sử. |
| Thành viên đề tài | Liên kết Sinh viên với đề tài | Một đến hai thành viên, có một đại diện. |
| Bài nộp | Một phiên bản đề cương hoặc khóa luận | Thuộc đề tài, có người nộp và các nhận xét. |
| Nhận xét | Kết quả phản hồi cho bài nộp | Có tác giả là Giảng viên; có thể yêu cầu chỉnh sửa. |
| Hội đồng | Nhóm chấm và lịch bảo vệ | Thuộc đợt, có chủ tịch/thư ký và nhận nhiều đề tài. |
| Mẫu phiếu và tiêu chí | Cấu hình cách chấm | Mẫu được gán cho vai trò chấm trong một đợt; tiêu chí có cấu trúc chung/con. |
| Phiếu điểm và chi tiết điểm | Điểm của một người chấm cho một Sinh viên | Gắn vai trò, mẫu phiếu, người chấm/người nhập và các tiêu chí. |
| Thông báo | Thông tin gửi tới người dùng | Có người nhận, có thể có người gửi; hỗ trợ trạng thái đã đọc. |
| Lịch sử đề tài | Dấu vết thay đổi nghiệp vụ | Gắn đề tài, người thực hiện (có thể là hệ thống), thời điểm và nội dung. |

## Quan hệ và bất biến quan trọng

- Một đề tài thuộc một đợt, có một giảng viên hướng dẫn và tối đa một nhóm đang đăng ký.
- Một đợt có nhiều đề tài/hội đồng và gán mẫu phiếu cho các vai trò chấm.
- Một bài nộp thuộc một đề tài; các phiên bản trước không bị ghi đè khi có lần nộp mới.
- Một Sinh viên có bốn kết quả chấm độc lập khi đủ điều kiện: hướng dẫn, phản biện, chủ tịch và thư ký.
- Lịch sử và thông báo bổ trợ cho mọi thay đổi, nhưng không được thay thế trạng thái nghiệp vụ chính.

Các thuộc tính vật lý, khoá, enum kỹ thuật, quan hệ xóa và migration chỉ được quyết định khi có đặc tả triển khai sau này.
