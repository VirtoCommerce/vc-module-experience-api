using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Commands
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, IdentityResult>
    {
        private readonly IMapper _mapper;
        private readonly IServiceProvider _services;

        public CreateUserCommandHandler(IMapper mapper, IServiceProvider services)
        {
            _mapper = mapper;
            _services = services;
        }

        public async Task<IdentityResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // UserManager<ApplicationUser> requires scoped service
            using (var scope = _services.CreateScope())
            {
                var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                return await _userManager.CreateAsync(request, request.Password);
            }
        }
    }
}
