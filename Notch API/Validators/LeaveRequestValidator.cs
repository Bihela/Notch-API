using FluentValidation;
using Notch_API.Models;

namespace Notch_API.Validators
{
    public class LeaveRequestValidator : AbstractValidator<LeaveRequest>
    {
        public LeaveRequestValidator()
        {
            RuleFor(lr => lr.EmployeeId)
                .GreaterThan(0)
                .WithMessage("EmployeeId must be greater than 0.");

            RuleFor(lr => lr.StartDate)
                .LessThanOrEqualTo(lr => lr.EndDate)
                .WithMessage("StartDate must be less than or equal to EndDate.")
                .Must(startDate => startDate.Date >= DateTime.Today)
                .WithMessage("StartDate cannot be in the past.");

            RuleFor(lr => lr.EndDate)
                .Must(endDate => endDate.Date >= DateTime.Today)
                .WithMessage("EndDate cannot be in the past.");

            RuleFor(lr => lr.Reason)
                .NotEmpty()
                .WithMessage("Reason is required.")
                .MaximumLength(500)
                .WithMessage("Reason cannot exceed 500 characters.");

            RuleFor(lr => lr.LeaveType)
                .IsInEnum()
                .WithMessage("Invalid leave type.");
        }
    }
}