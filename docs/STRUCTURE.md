# Cấu trúc tài liệu QLKLTN

Tài liệu này là bản đồ của `docs/`, chỉ nêu vị trí và mục đích. Bộ tài liệu cố ý giữ gọn: chưa tạo `plans/`, `specs/` hay `legacy/` vì chưa cần dùng.

```text
docs/
├── DESIGN/
│   ├── 01-sitemap.md
│   ├── 02-thiet-ke-dung-chung.md
│   ├── 03-dac-ta-man-hinh.md
│   └── Sitemap.png
├── architecture/
│   ├── DOMAIN_MODEL.md
│   └── SYSTEM_CONTEXT.md
├── decisions/
│   ├── PDR-001-vai-tro-va-quy-trinh-co-kiem-soat.md
│   └── PDR-002-xac-thuc-va-lo-trinh-ma-nguon.md
├── product/
│   ├── GLOSSARY.md
│   ├── ROLES.md
│   ├── SCOPE.md
│   └── VISION.md
├── requirements/
│   ├── BUSINESS_RULES.md
│   ├── NON_FUNCTIONAL.md
│   └── WORKFLOWS.md
├── CONTENT.md
├── FEATURE.md
├── NEON.md
├── OPEN_QUESTIONS.md
├── PROJECT_SPEC.md
└── README.md
```

| Khu vực | Chức năng |
| --- | --- |
| Cấp gốc | Điểm vào, phạm vi, danh mục chức năng và câu hỏi chưa chốt. |
| `product/` | Ngôn ngữ chung để xác định sản phẩm và vai trò trước khi nói về kỹ thuật. |
| `requirements/` | Luồng và quy tắc dùng để thiết kế, hiện thực và kiểm thử. |
| `architecture/` | Khái niệm miền, tác nhân và ranh giới hệ thống; không phải sơ đồ triển khai chi tiết. |
| `decisions/` | Các lựa chọn đã có căn cứ rõ ràng, không chứa giả định mới. |
| `DESIGN/` | Tài liệu bàn giao giao diện hiện có, được giữ tách khỏi yêu cầu nghiệp vụ để tránh nhầm chi tiết UI với quy tắc sản phẩm. |
| `NEON.md` | Hướng dẫn kỹ thuật hiện có của repo, giữ nguyên vì được README gốc tham chiếu. |
| `README.md` | Con trỏ ngắn về `CONTENT.md` và vị trí tài liệu thiết kế giao diện. |
