# Bàn giao giao diện QLKLTN

Tài liệu cho người dev FE chính nhận giao diện từ bản thiết kế mẫu (canvas). Nguồn thiết kế: canvas "QLKLTN – Sitemap & UI mẫu" (https://claude.ai/artifact/J23aEMjV4mBv6DZ2mB8HWr), cập nhật đến bản 227 ngày 22/10/2026.

| File | Nội dung |
| --- | --- |
| [01-sitemap.md](01-sitemap.md) | Sơ đồ trang phân tầng 0–3, route gợi ý theo Next.js App Router, màn nào dùng chung khung nào |
| [02-thiet-ke-dung-chung.md](02-thiet-ke-dung-chung.md) | Màu, phông, cỡ chữ, bo góc, khoảng cách, thành phần lặp lại (sidebar, thanh trên, chip, bảng, popup, trạng thái trống và lỗi) |
| [03-dac-ta-man-hinh.md](03-dac-ta-man-hinh.md) | Đặc tả từng màn: người dùng, trạng thái cần dựng (khớp prop `view` trên canvas), hành vi, quy tắc nghiệp vụ |

## Cách đọc canvas

- Mỗi màn là một artboard `project/<Tên>.dc.html`. Nhiều màn có prop `view` (hoặc `phase`, `tab`, `scenario`...) để chuyển giữa các trạng thái: danh sách ở file 03, mỗi trạng thái là một case nên dựng và test.
- Dữ liệu trong canvas là dữ liệu mẫu (tên, mã, ngày giả, placeholder dạng `[MSSV]`). Không có API: mọi hành vi chạy bằng state cục bộ để minh họa.
- Màn "Đăng nhập" đã được dựng trong repo ở `/login` (`frontend/src/features/auth`).

## Việc cần làm khi nối dữ liệu thật

Gom các ghi chú `NOTE(code)` và quyết định chưa có trong mã mẫu:

1. **Bộ lọc Đợt:** Đề tài của tôi và Đề xuất từ SV (giảng viên) có ô lọc Đợt. Trên canvas ô này chỉ là điều khiển mẫu. Khi nối dữ liệu, hai trang dùng chung một bộ lọc đợt, và vào từ thẻ đợt ở Trang chủ thì chọn sẵn đúng đợt đó (truyền mã đợt qua query string hoặc route, ví dụ `?dot=<id>`).
2. **Trang chủ:** mở ra danh sách thẻ các đợt của người dùng; bấm một thẻ mới vào nội dung chi tiết đợt (việc cần làm, Bảng thông báo, Gantt). Nút "Tất cả đợt" quay về danh sách. Đây là bản thử nghiệm, người dùng chưa chốt hẳn.
3. **Link giữa trang không truyền tham số** trên canvas (nên chỉ có preset): khi dựng thật, các link từ danh sách vào chi tiết, vào Không gian đề tài, Gửi thông báo cho một nhóm đều cần tham số (id đề tài, id nhóm, id đợt).
4. **Sidebar cố định khi cuộn:** sidebar `position: sticky; top: 0; height: 100vh` với phần nội dung cuộn riêng; thẻ tài khoản ở đáy sidebar.
5. **Quyền theo vai trò** (Sinh viên, Giảng viên, Giáo vụ; giảng viên có thêm vai trò HD, PB, thành viên hội đồng theo từng đề tài) phải kiểm tra ở backend, giao diện chỉ ẩn/hiện.
6. **Quy ước repo** (xem `AGENTS.md`): đối chiếu hợp đồng API cả hai phía trước khi code, không đoán endpoint.
