using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.XProfile.Extensions;
using VirtoCommerce.ExperienceApiModule.XProfile.Models;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Queries
{
    public class PasswordValidationQueryHandler : IQueryHandler<PasswordValidationQuery, IdentityResultResponse>
    {
        private readonly Func<UserManager<ApplicationUser>> _userManagerFactory;

        public PasswordValidationQueryHandler(Func<UserManager<ApplicationUser>> userManagerFactory)
        {
            _userManagerFactory = userManagerFactory;
        }

        public virtual async Task<IdentityResultResponse> Handle(PasswordValidationQuery request, CancellationToken cancellationToken)
        {
            var result = new IdentityResultResponse
            {
                Errors = new List<IdentityErrorInfo>(),
                Succeeded = true,
            };

            using var userManager = _userManagerFactory();

            foreach (var passwordValidator in userManager.PasswordValidators)
            {
                var validationResult = await passwordValidator.ValidateAsync(userManager, null, request.Password);
                result.Succeeded &= validationResult.Succeeded;

                result.Errors.AddRange(validationResult.Errors.Select(x => x.MapToIdentityErrorInfo()));
            }

            return result;
        }
    }
}
