using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Queries
{
    public class GetRoleQueryHandler : IQueryHandler<GetRoleQuery, Role>
    {
        private readonly IServiceProvider _services;

        public GetRoleQueryHandler(IServiceProvider services)
        {
            _services = services;
        }

        public virtual async Task<Role> Handle(GetRoleQuery request, CancellationToken cancellationToken)
        {
            using var scope = _services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
            return await roleManager.FindByNameAsync(request.RoleName);
        }
    }
}
