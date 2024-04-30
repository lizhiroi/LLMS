using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace LLMS.Validators
{
    public class TenantValidator : AbstractValidator<tenant>
    {
        public TenantValidator()
        {
            RuleFor(t => t).NotNull().WithMessage("Tenant object cannot be null.");

            RuleFor(t => t.email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email address format.");

            RuleFor(t => t.first_name)
                .NotEmpty().WithMessage("First name is required.");

            RuleFor(t => t.last_name)
                .NotEmpty().WithMessage("Last name is required.");

            RuleFor(t => t.street_number)
                .NotEmpty().WithMessage("Street number is required.");

            RuleFor(t => t.street_name)
                .NotEmpty().WithMessage("Street name is required.");

            RuleFor(t => t.city_name)
                .NotEmpty().WithMessage("City name is required.");

            RuleFor(t => t.postcode)
                .NotEmpty().WithMessage("Postcode is required.");

            RuleFor(t => t.province)
                .NotEmpty().WithMessage("Province is required.");

            RuleFor(t => t.phone_number)
                .NotEmpty().WithMessage("Phone number is required.");
                
        }
    }

    public class LeaseValidator : AbstractValidator<leas>
    {
        public LeaseValidator()
        {
            RuleFor(l => l.property_id).NotEmpty();
            RuleFor(l => l.tenant_id).NotEmpty();
            RuleFor(l => l.start_date).NotEmpty();
            RuleFor(l => l.end_date).NotEmpty().GreaterThan(l => l.start_date).WithMessage("End date must be after start date");
            RuleFor(l => l.rent_amount).NotEmpty().GreaterThan(0).WithMessage("Rent amount must be greater than 0");
            RuleFor(l => l.rent_amount).Must(BeValidDecimal).WithMessage("Rent amount must be a valid number");
            RuleFor(l => l.lease_clauses).NotEmpty();
            RuleFor(l => l.payment_due_day).NotEmpty().InclusiveBetween(1, 31);
            // Add more validation rules as needed
        }

        // Custom validator method to check if rent_amount is a valid decimal number
        private bool BeValidDecimal(decimal rentAmount)
        {
            return decimal.TryParse(rentAmount.ToString(), out _);
        }
    }
}
