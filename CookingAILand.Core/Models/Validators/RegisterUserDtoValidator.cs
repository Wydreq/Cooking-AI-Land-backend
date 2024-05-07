using CookingAILand.Core.DAL.Persistence;
using CookingAILand.Core.Entities;
using FluentValidation;

namespace CookingAILand.Models.Validators;

public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserDtoValidator(CookingDbContext dbContext)
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).MinimumLength(6);
        RuleFor(x => x.ConfirmPassword).Equal(e => e.Password);
        RuleFor(x => x.Email).Custom((value, context) =>
        {
            var emailInUse = dbContext.Users.Any(u => u.Email == value);
            if (emailInUse)
            {
                context.AddFailure("Email", "Email is already in use");
            }
        });
        RuleFor(x => x.DateOfBirth).Custom((value, context) =>
        {
            if (value == null)
            {
                context.AddFailure("DateOfBirth", "Date of birth is required.");
                return;
            }

            var age = DateTime.Today.Year - value.Year;
            if (value > DateTime.Today.AddYears(-age))
            {
                age--;
            }

            if (age < 13)
            {
                context.AddFailure("DateOfBirth", "You must be at least 13 years old");
            } 
        }).NotEmpty();
    }
}