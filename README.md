# Notch API

This project is an Employee Management API built using **ASP.NET Core** and **Entity Framework Core**. It manages employees, departments, attendance, job performance, and salaries. The API supports CRUD operations for each of these entities, with additional features such as calculating attendance hours and handling salary calculations.

---

## Table of Contents
- [Features](#features)
- [Installation](#installation)
- [Controllers](#controllers)
  - [Attendance Controller](#attendance-controller)
  - [Department Controller](#department-controller)
  - [Employee Controller](#employee-controller)
  - [LeaveRequest Controller](#leaverequest-controller)
  - [Salary Controller](#salary-controller)
- [Database Models](#database-models)
- [Technologies](#technologies)
- [Usage](#usage)
- [API Endpoints](#api-endpoints)
  - [Attendance Endpoints](#attendance-endpoints)
  - [Department Endpoints](#department-endpoints)
  - [Employee Endpoints](#employee-endpoints)
  - [LeaveRequest Endpoints](#leaverequest-endpoints)
  - [Salary Endpoints](#salary-endpoints)

---

## Project Overview

The **Notch API** provides an efficient way to manage employee records, departments, and associated data such as attendance, job performance, and salaries. It features:

- **CRUD operations** for employees, departments, attendance, and salary records.
- Calculating **total working hours** for employee attendance.
- **Net salary calculation** for employees based on their basic salary, bonuses, and deductions.
- **Department management**, including employee relationships.

---

## Technologies Used

- **ASP.NET Core 6:** Web API framework.
- **Entity Framework Core:** ORM for database access.
- **SQL Server:** Database used for data storage.
- **Swagger:** API documentation and testing.
- **CORS:** Configured to allow cross-origin requests.

---

## Project Structure

**Notch_API**

* **Configurations**
    * `DepartmentConfiguration.cs`
* **Controllers**
  * **Attendance Controller:**
    Manages employee attendance records, including present/absent status, late arrivals, and in/out times.
  * **Department Controller:**
    Handles CRUD operations for departments, including assigning employees to departments.
  * **Employee Controller:**
    Provides endpoints to manage employee data, including linking employees to departments.
  * **LeaveRequest Controller:**
    Handles employee leave requests, with functionality to approve, reject, and view requests based on status.
  * **Salary Controller:**
    Calculates and stores employee salary details, including net salary after bonuses and deductions.
* **Data**
    * `EmployeeManagementContext.cs`
* **Models**
    * `Attendance.cs`
    * `Department.cs`
    * `Employee.cs`
    * `LeaveRequest.cs`
    * `Salary.cs`
* `Program.cs`
* `appsettings.json`

## Getting Started

### Prerequisites

- .NET 6 SDK or later
- SQL Server (or SQL Server Express)
- Visual Studio 2022 (or any C# IDE)

### Installation

1. **Clone the repository:**

    ```bash
    git clone https://github.com/Bihela/Notch-API.git
    cd Notch-API
    ```

2. **Restore NuGet packages:**

    ```bash
    dotnet restore
    ```

3. **Update Database Connection:**

    Update the `DefaultConnection` string in `appsettings.json` to match your SQL Server configuration:

    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Server=your_server;Database=EmployeeManagementDb;Trusted_Connection=True;"
      }
    }
    ```

4. **Build the project:**

    ```bash
    dotnet build
    ```

### Database Setup

1. **Apply Migrations:**

    Run the following command to apply database migrations and create the database schema:

    ```bash
    dotnet ef database update
    ```

2. **Seed Data (Optional):**

    You can seed initial data using a seed method (if available) or insert data manually via SQL Server Management Studio.

---

## API Endpoints

| Method | Endpoint                         | Description                            |
|--------|----------------------------------|----------------------------------------|
| GET    | /api/Employee                    | Retrieve all employees                 |
| GET    | /api/Employee/{id}               | Retrieve an employee by ID             |
| POST   | /api/Employee                    | Create a new employee                  |
| PUT    | /api/Employee/{id}               | Update an existing employee            |
| DELETE | /api/Employee/{id}               | Delete an employee                     |
| GET    | /api/Department                  | Retrieve all departments               |
| GET    | /api/Department/{id}             | Retrieve a department by ID            |
| POST   | /api/Department                  | Create a new department                |
| GET    | /api/Attendance/CalculateHours/{id} | Calculate working hours for attendance |
| POST   | /api/Salary                      | Add a new salary entry                 |

Visit the Swagger UI at `/swagger/index.html` to explore all the available endpoints and test them.

---

## Configurations

- **CORS:** Configured to allow all origins, headers, and methods. Modify the CORS policy in `Program.cs` as needed.
- **Swagger:** Enabled for API documentation. Accessible in development mode.

---

## Contributing

Contributions are welcome! Please create a pull request or open an issue if you have any improvements or bug fixes.

---

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---
