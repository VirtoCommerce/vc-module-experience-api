using System;
using System.Collections.Generic;
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

        public PasswordValidationQueryHandler(Func<UserManager<ApplicationUser>> userManagerFactory)
        {
            _userManagerFactory = userManagerFactory;
        }

        public async Task<PasswordValidationResponse> Handle(PasswordValidationQuery request, CancellationToken cancellationToken)
        {
            var result = new PasswordValidationResponse
            {
                ErrorCodes = new List<string>(),
                Succeeded = true,
            };

            using var userManager = _userManagerFactory();

            foreach (var passwordValidator in userManager.PasswordValidators)
            {
                var validationResult = await passwordValidator.ValidateAsync(userManager, null, request.Password);

                // If at least one validator fails, result should be failed
                if (!validationResult.Succeeded)
                {
                    result.Succeeded = validationResult.Succeeded;
                }

                ((List<string>)result.ErrorCodes).AddRange(validationResult.Errors.Select(x => x.Code));

            }

            return result;
        }
    }
}
