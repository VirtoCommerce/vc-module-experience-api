using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Queries
{
    public class CheckEmailUniquenessQueryHandler : IQueryHandler<CheckEmailUniquenessQuery, CheckEmailUniquenessResponse>
    {
        private readonly Func<UserManager<ApplicationUser>> _userManagerFactory;

        public CheckEmailUniquenessQueryHandler(Func<UserManager<ApplicationUser>> userManagerFactory)
        {
            _userManagerFactory = userManagerFactory;
        }

        public virtual async Task<CheckEmailUniquenessResponse> Handle(CheckEmailUniquenessQuery request, CancellationToken cancellationToken)
        {
            var result = new CheckEmailUniquenessResponse();

            using var userManager = _userManagerFactory();

            var query = userManager.Users;

            result.IsUnique = !await query.AnyAsync(x => x.Email == request.Email, cancellationToken);

            return result;
        }
    }
}
