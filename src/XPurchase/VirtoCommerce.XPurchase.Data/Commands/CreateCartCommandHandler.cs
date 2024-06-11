using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.XPurchase.Core;
using VirtoCommerce.XPurchase.Core.Commands;
using VirtoCommerce.XPurchase.Core.Commands.BaseCommands;
using VirtoCommerce.XPurchase.Core.Services;

namespace VirtoCommerce.XPurchase.Data.Commands
{
    public class CreateCartCommandHandler : CartCommandHandler<CreateCartCommand>
    {
        private readonly Func<UserManager<ApplicationUser>> _userManagerFactory;
        private readonly IMemberService _memberService;

        public CreateCartCommandHandler(
            ICartAggregateRepository cartAggrRepository,
            Func<UserManager<ApplicationUser>> userManagerFactory,
            IMemberService memberService)
            : base(cartAggrRepository)
        {
            _userManagerFactory = userManagerFactory;
            _memberService = memberService;
        }

        public override async Task<CartAggregate> Handle(CreateCartCommand request, CancellationToken cancellationToken)
        {
            var cartAggregate = await CreateNewCartAggregateAsync(request);

            cartAggregate.Cart.OrganizationId ??= await GetOrganizationId(request.UserId);

            return await SaveCartAsync(cartAggregate);
        }

        protected virtual async Task<string> GetOrganizationId(string userId)
        {
            Contact contact = null;

            using var userManager = _userManagerFactory();
            var user = await userManager.FindByIdAsync(userId);

            if (!string.IsNullOrEmpty(user?.MemberId))
            {
                contact = await _memberService.GetByIdAsync(user.MemberId) as Contact;
            }

            return contact?.Organizations?.FirstOrDefault();
        }
    }
}
