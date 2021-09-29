using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, IdentityResult>
    {
        private readonly IServiceProvider _services;


        public UpdateRoleCommandHandler(IServiceProvider services)
        {
            _services = services;
        }

        public virtual async Task<IdentityResult> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            IdentityResult result;

            using var scope = _services.CreateScope();
            var _roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
            var roleExists = string.IsNullOrEmpty(request.Role.Id) ?
                await _roleManager.RoleExistsAsync(request.Role.Name) :
                await _roleManager.FindByIdAsync(request.Role.Id) != null;
            if (!roleExists)
            {
                result = await _roleManager.CreateAsync(request.Role);
            }
            else
            {
                result = await _roleManager.UpdateAsync(request.Role);
            }

            return result;
        }
    }
}
