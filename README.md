# SWP391BackEnd - EV Cost Sharing Platform (Backend)

Backend API cho nền tảng **chia sẻ chi phí xe điện** (co-ownership / nhóm sử dụng chung xe), xây dựng bằng **ASP.NET Core Web API + Entity Framework Core + PostgreSQL**.

## 1) Mục tiêu dự án

Hệ thống hỗ trợ:
- Quản lý tài khoản, phân quyền, xác thực JWT.
- Quản lý nhóm đồng sở hữu xe điện.
- Hợp đồng điện tử và quy trình ký hợp đồng.
- Quản lý yêu cầu dịch vụ, job kỹ thuật, chi phí nhóm.
- Phân bổ hóa đơn thành viên, thanh toán và giao dịch.
- Theo dõi quota sử dụng và sự kiện chuyến đi.
- Thông báo trong hệ thống.

## 2) Kiến trúc tổng quan

Mô hình phân lớp theo solution:

- `SWP391BackEnd`: API layer (controllers, Program startup, Swagger, middleware).
- `Service`: Business logic layer (use-cases, validation, orchestration).
- `Repository`: Data access abstraction cho domain.
- `DataAccessLayer`: `AppDbContext`, migrations, cấu hình EF Core.
- `BusinessObject`: Entities + DTOs.

## 3) Cấu trúc thư mục
<img width="213" height="529" alt="image" src="https://github.com/user-attachments/assets/7be484a9-5f9e-495c-a9d3-fb91d14dabe5" />

## 4) Database diagram: https://dbdiagram.io/d/68d00712960f6d821a16e926
