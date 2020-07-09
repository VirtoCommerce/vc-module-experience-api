using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Queries
{
    public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, ApplicationUser>
    {
        private readonly IServiceProvider _services;

        public GetUserByIdQueryHandler(IServiceProvider services)
        {
            _services = services;
        }

        public async Task<ApplicationUser> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            // UserManager<ApplicationUser> requires scoped service
            using (var scope = _services.CreateScope())
            {
                var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                return await _userManager.FindByIdAsync(request.Id);
            }
        }
    }
}
