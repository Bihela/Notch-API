using Microsoft.EntityFrameworkCore;
using Notch_API.Data;
using FluentValidation;
using Notch_API.Models;
using Notch_API.Validators;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Ignore cycles to prevent reference loop errors
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

        // Use camelCase naming convention for JSON properties
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

        // Add JSON options for enum serialization
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Register validators for dependency injection
builder.Services.AddTransient<IValidator<Employee>, EmployeeValidator>();
builder.Services.AddTransient<IValidator<Department>, DepartmentValidator>();
builder.Services.AddTransient<IValidator<Attendance>, AttendanceValidator>();
builder.Services.AddTransient<IValidator<LeaveRequest>, LeaveRequestValidator>(); // Register LeaveRequestValidator

// Add the DbContext to the services collection
builder.Services.AddDbContext<EmployeeManagementContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
                .AllowAnyOrigin() // Allow any origin
                .AllowAnyMethod() // Allow any HTTP method
                .AllowAnyHeader(); // Allow any header
        });
});

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use the CORS policy
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
