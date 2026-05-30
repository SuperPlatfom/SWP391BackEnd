# SWP391BackEnd - EV Cost Sharing Platform (Backend)

Backend API for an Electric Vehicle Cost Sharing Platform (EV co-ownership / shared vehicle usage), built with **ASP.NET Core Web API + Entity Framework Core + PostgreSQL**.

## 1) Project Objective

The system supports:

- User account management, role-based authorization, and JWT authentication.
- Management of EV co-ownership groups.
- Electronic contracts and contract signing workflows.
- Service request management, technical job tracking, and group expense management.
- Member bill allocation, payment processing, and transaction management.
- Usage quota tracking and trip event monitoring.
- In-app notification system.

## 2) System Architecture

The solution follows a layered architecture:

- `SWP391BackEnd`: API layer (controllers, Program startup, Swagger, middleware).
- `Service`: Business logic layer (use-cases, validation, orchestration).
- `Repository`: Data access abstraction cho domain.
- `DataAccessLayer`: `AppDbContext`, migrations, cấu hình EF Core.
- `BusinessObject`: Entities + DTOs.

## 3) Project Structure
<img width="213" height="529" alt="image" src="https://github.com/user-attachments/assets/7be484a9-5f9e-495c-a9d3-fb91d14dabe5" />

## 4) Database diagram: https://dbdiagram.io/d/68d00712960f6d821a16e926
