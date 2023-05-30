using FluentValidation;
using static HealthCheckSample.Controllers.UserController;

namespace HealthCheckSample.Models.Validator
{
    /// <summary>
    /// UserUpdate 驗證規則
    /// </summary>
    public class UserUpdateValidator: AbstractValidator<UserUpdateRequest>
    {
        public UserUpdateValidator()
        {
            RuleFor(x => x.id).NotNull();
            RuleFor(x => x.password).NotNull().MinimumLength(6).MaximumLength(8);
            RuleFor(x => x.confirmPassword).NotNull().MinimumLength(6).MaximumLength(8);
            RuleFor(x => x.confirmPassword).Equal(x => x.password).WithMessage("confirmPassword的值須和password一致");
            RuleFor(x => x.enable).NotNull().InclusiveBetween(0, 1);
        }
    }
}
