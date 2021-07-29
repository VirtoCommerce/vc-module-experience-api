using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries
{
    public class PasswordValidationQueryHandler : IQueryHandler<PasswordValidationQuery, PasswordValidationResponse>
    {
        private readonly Func<UserManager<ApplicationUser>> _userManagerFactory;
        private readonly Func<IPasswordValidator<ApplicationUser>> _passwordValidatorFactory;

        public PasswordValidationQueryHandler(
            Func<IPasswordValidator<ApplicationUser>> passwordValidatorFactory,
            Func<UserManager<ApplicationUser>> userManagerFactory)
        {
            _passwordValidatorFactory = passwordValidatorFactory;
            _userManagerFactory = userManagerFactory;
        }

        public async Task<PasswordValidationResponse> Handle(PasswordValidationQuery request, CancellationToken cancellationToken)
        {
            using var userManager = _userManagerFactory();
            var passwordValidator = _passwordValidatorFactory();

            var validationResult = await passwordValidator.ValidateAsync(userManager, null, request.Password);

            var result = new PasswordValidationResponse
            {
                Succeeded = validationResult.Succeeded,
                ErrorCodes = validationResult.Errors.Select(x => x.Code).ToArray(),
            };

            return result;
        }
    }
}
