using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VirtoCommerce.ExperienceApiModule.XProfile.Requests;
using VirtoCommerce.ExperienceApiModule.XProfile.Services;
using VirtoCommerce.OrdersModule.Core.Services;

namespace VirtoCommerce.ExperienceApi.ProfileModule.Data.Handlers
{
    public class LoadProfileRequestHandler : IRequestHandler<LoadProfileRequest, LoadProfileResponse>
    {
        private readonly ICustomerOrderSearchService _orderSearchService;
        private readonly IMemberServiceX _memberService;
        private readonly IServiceProvider _services;

        public LoadProfileRequestHandler(ICustomerOrderSearchService orderSearchService, IMemberServiceX memberService, IServiceProvider services)
        {
            _orderSearchService = orderSearchService;
            _memberService = memberService;
            _services = services;
        }

        public async Task<LoadProfileResponse> Handle(LoadProfileRequest request, CancellationToken cancellationToken)
        {
            var result = new LoadProfileResponse();
            var profile = await _memberService.GetProfileByIdAsync(request.Id);

            result.Results.Add(request.Id, profile);
            return result;
        }
    }
}
