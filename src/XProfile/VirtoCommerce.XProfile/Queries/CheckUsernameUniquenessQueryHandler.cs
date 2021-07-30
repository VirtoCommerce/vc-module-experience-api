using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Queries
{
    public class CheckUsernameUniquenessQueryHandler : IQueryHandler<CheckUsernameUniquenessQuery, CheckUsernameUniquenessResponse>
    {
        private readonly Func<UserManager<ApplicationUser>> _userManagerFactory;

        public CheckUsernameUniquenessQueryHandler(Func<UserManager<ApplicationUser>> userManagerFactory)
        {
            _userManagerFactory = userManagerFactory;
        }

        public virtual async Task<CheckUsernameUniquenessResponse> Handle(CheckUsernameUniquenessQuery request, CancellationToken cancellationToken)
        {
            var result = new CheckUsernameUniquenessResponse();

            using var userManager = _userManagerFactory();

            var query = userManager.Users;

            result.IsUnique = !await query.AnyAsync(x => x.UserName == request.Username, cancellationToken);

            return result;
        }
    }
}
