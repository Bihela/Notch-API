using FluentValidation;
using Notch_API.Models;


namespace Notch_API.Validators
{
    public class EmployeeValidator : AbstractValidator<Employee>
    {
        public EmployeeValidator()
        {
            RuleFor(e => e.Name)
                .NotEmpty().WithMessage("Employee name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(e => e.Position)
                .NotEmpty().WithMessage("Position is required.")
                .MaximumLength(50).WithMessage("Position cannot exceed 50 characters.");

            RuleFor(e => e.DepartmentID)
                .NotEmpty().WithMessage("Department ID is required.");

            RuleFor(e => e.DateOfJoining)
                .NotEmpty().WithMessage("Date of joining is required.");

            RuleFor(e => e.EmailAddress)
                .NotEmpty().WithMessage("Email address is required.")
                .EmailAddress().WithMessage("Invalid email address.");

            RuleFor(e => e.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?[1-9][0-9\s\-]{1,14}$").WithMessage("Invalid phone number.");

        }
    }
}