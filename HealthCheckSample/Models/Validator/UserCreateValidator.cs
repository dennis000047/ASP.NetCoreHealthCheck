using FluentValidation;
using static HealthCheckSample.Controllers.UserController;

namespace HealthCheckSample.Models.Validator
{
    /// <summary>
    /// UserCreate 驗證規則
    /// </summary>
    public class UserCreateValidator: AbstractValidator<UserCreateRequset>
    {
        public UserCreateValidator()
        {
            RuleFor(x => x.account).NotNull().MinimumLength(6).MaximumLength(8);
            RuleFor(x => x.password).NotNull().MinimumLength(6).MaximumLength(8);
            RuleFor(x => x.enable).NotNull().InclusiveBetween(0, 1);
        }
    }
}
