using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.XProfile.Models;
using VirtoCommerce.ExperienceApiModule.XProfile.Requests;
using VirtoCommerce.OrdersModule.Core.Model.Search;
using VirtoCommerce.OrdersModule.Core.Services;
using VirtoCommerce.Platform.Core.Security;

namespace VirtoCommerce.ExperienceApi.ProfileModule.Data.Handlers
{
    public class LoadProfileRequestHandler : IRequestHandler<LoadProfileRequest, LoadProfileResponse>
    {
        private readonly ICustomerOrderSearchService _orderSearchService;
        private readonly IMemberService _memberService;
        private readonly IServiceProvider _services;

        public LoadProfileRequestHandler(ICustomerOrderSearchService orderSearchService, IMemberService memberService, IServiceProvider services)
        {
            _orderSearchService = orderSearchService;
            _memberService = memberService;
            _services = services;
        }

        public async Task<LoadProfileResponse> Handle(LoadProfileRequest request, CancellationToken cancellationToken)
        {
            var result = new LoadProfileResponse();

            // UserManager<ApplicationUser> requires scoped service
            using (var scope = _services.CreateScope())
            {
                var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var user = await _userManager.FindByIdAsync(request.Id);
                if (user != null)
                {
                    var orderSearchResult = await _orderSearchService.SearchCustomerOrdersAsync(new CustomerOrderSearchCriteria
                    {
                        CustomerId = user.Id,
                        Take = 0
                    });

                    var profile = new Profile();
                    profile.IsFirstTimeBuyer = orderSearchResult.TotalCount == 0;
                    profile.User = user;

                    //Load the associated contact
                    if (user.MemberId != null)
                    {
                        profile.Contact = await _memberService.GetByIdAsync(user.MemberId, null, nameof(Contact)) as Contact;
                    }

                    result.Results.Add(request.Id, profile);
                }
            }

            return result;
        }
    }
}
