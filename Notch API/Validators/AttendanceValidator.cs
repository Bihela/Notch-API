using FluentValidation;
using Notch_API.Models;

namespace Notch_API.Validators
{
    public class AttendanceValidator : AbstractValidator<Attendance>
    {
        public AttendanceValidator()
        {
            RuleFor(a => a.EmployeeId)
                .GreaterThan(0).WithMessage("Employee ID must be greater than zero.");

            RuleFor(a => a.Status)
                .NotEmpty().WithMessage("Status is required.")
                .Must(status => new[] { "Present", "Not Present", "Need to Attend" }.Contains(status))
                .WithMessage("Status must be one of the following values: Present, Not Present, Need to Attend.");
        }
    }
}
