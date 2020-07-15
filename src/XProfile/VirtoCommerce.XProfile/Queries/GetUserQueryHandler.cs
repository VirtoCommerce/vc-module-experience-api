using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.XPurchase.Validators;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Queries
{
    public class GetUserQueryHandler : IQueryHandler<GetUserQuery, ApplicationUser>
    {
        private readonly Func<UserManager<ApplicationUser>> _userManagerFactory;

        public GetUserQueryHandler(Func<UserManager<ApplicationUser>> userManager)
        {
            _userManagerFactory = userManager;
        }

        public async Task<ApplicationUser> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            ApplicationUser result = default;

            await new UserQueryValidator().ValidateAndThrowAsync(request);

            using (var userManager = _userManagerFactory())
            {
                if (!request.Id.IsNullOrEmpty())
                {
                    result = await userManager.FindByIdAsync(request.Id);
                }

                if (!request.Email.IsNullOrEmpty())
                {
                    result = await userManager.FindByEmailAsync(request.Email);
                }

                if (!request.UserName.IsNullOrEmpty())
                {
                    result = await userManager.FindByNameAsync(request.UserName);
                }

                if (!request.LoginProvider.IsNullOrEmpty() && !request.ProviderKey.IsNullOrEmpty())
                {
                    result = await userManager.FindByLoginAsync(request.LoginProvider, request.ProviderKey);
                }
            }

            return result;
        }
    }
}
