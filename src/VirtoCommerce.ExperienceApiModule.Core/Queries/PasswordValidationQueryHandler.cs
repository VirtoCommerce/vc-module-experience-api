using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Security;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries
{
    public class PasswordValidationQueryHandler : IQueryHandler<PasswordValidationQuery, PasswordValidationResponse>
    {
        private readonly Func<UserManager<ApplicationUser>> _userManagerFactory;

        public PasswordValidationQueryHandler(Func<UserManager<ApplicationUser>> userManagerFactory)
        {
            _userManagerFactory = userManagerFactory;
        }

        public virtual async Task<PasswordValidationResponse> Handle(PasswordValidationQuery request, CancellationToken cancellationToken)
        {
            var result = new PasswordValidationResponse
            {
                Errors = new List<IdentityErrorInfo>(),
                Succeeded = true,
            };

            using var userManager = _userManagerFactory();

            foreach (var passwordValidator in userManager.PasswordValidators)
            {
                var validationResult = await passwordValidator.ValidateAsync(userManager, null, request.Password);
                result.Succeeded &= validationResult.Succeeded;

                result.Errors.AddRange(validationResult.Errors.Select(MapToIdentityErrorInfo));
            }

            return result;
        }

        protected virtual IdentityErrorInfo MapToIdentityErrorInfo(IdentityError x)
        {
            var error = new IdentityErrorInfo { Code = x.Code, Description = x.Description };
            if (x is CustomIdentityError customIdentityError)
            {
                error.ErrorParameter = customIdentityError.ErrorParameter;
            }

            return error;
        }
    }
}
