using FluentValidation;
using Notch_API.Models;

namespace Notch_API.Validators
{
    public class DepartmentValidator : AbstractValidator<Department>
    {
        public DepartmentValidator()
        {
            RuleFor(d => d.DepartmentName)
                .NotEmpty().WithMessage("Department name is required.")
                .MaximumLength(100).WithMessage("Department name cannot exceed 100 characters.");

            RuleFor(d => d.ManagerId)
                .GreaterThan(0).WithMessage("Manager ID must be a positive number.");

            RuleForEach(d => d.Employees)
                .SetValidator(new EmployeeValidator());
        }
    }
}
