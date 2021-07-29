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
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IPasswordValidator<ApplicationUser> _passwordValidator;

        public PasswordValidationQueryHandler(
            Func<IPasswordValidator<ApplicationUser>> passwordValidator,
            Func<SignInManager<ApplicationUser>> signInManager)
        {
            _signInManager = signInManager();
            _passwordValidator = passwordValidator();
        }

        public async Task<PasswordValidationResponse> Handle(PasswordValidationQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await _passwordValidator.ValidateAsync(_signInManager.UserManager, null, request.Password);

            var result = new PasswordValidationResponse
            {
                Succeeded = validationResult.Succeeded,
                ErrorCodes = validationResult.Errors.Select(x => x.Code).ToArray(),
            };

            return result;
        }
    }
}
