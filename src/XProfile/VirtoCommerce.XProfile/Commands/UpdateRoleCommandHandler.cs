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

        public async Task<IdentityResult> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            IdentityResult result;

            using var scope = _services.CreateScope();
            var _roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
            var roleExists = string.IsNullOrEmpty(request.Id) ?
                await _roleManager.RoleExistsAsync(request.Name) :
                await _roleManager.FindByIdAsync(request.Id) != null;
            if (!roleExists)
            {
                result = await _roleManager.CreateAsync(request);
            }
            else
            {
                result = await _roleManager.UpdateAsync(request);
            }

            return result;
        }
    }
}
