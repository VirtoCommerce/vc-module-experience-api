using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PipelineNet.Middleware;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.TaxModule.Core.Model;

namespace VirtoCommerce.XProfile.Middlewares
{
    public class LoadUserToEvalContextMiddleware : IAsyncMiddleware<PromotionEvaluationContext>, IAsyncMiddleware<PriceEvaluationContext>, IAsyncMiddleware<TaxEvaluationContext>
    {
        private readonly IMapper _mapper;
        private readonly IMemberResolver _memberIdResolver;
        public LoadUserToEvalContextMiddleware(IMapper mapper, IMemberResolver memberIdResolver)
        {
            _mapper = mapper;
            _memberIdResolver = memberIdResolver;
        }

        public async Task Run(PromotionEvaluationContext parameter, Func<PromotionEvaluationContext, Task> next)
        {
            if (!string.IsNullOrEmpty(parameter.CustomerId))
            {
                await InnerSetShopperDataFromMember(parameter, parameter.CustomerId);
            }
            await next(parameter);
        }

        public async Task Run(PriceEvaluationContext parameter, Func<PriceEvaluationContext, Task> next)
        {
            if (!string.IsNullOrEmpty(parameter.CustomerId))
            {
                await InnerSetShopperDataFromMember(parameter, parameter.CustomerId);
            }
            await next(parameter);
        }

        public async Task Run(TaxEvaluationContext parameter, Func<TaxEvaluationContext, Task> next)
        {
            if (!string.IsNullOrEmpty(parameter.CustomerId))
            {
                var member = await _memberIdResolver.ResolveMemberByIdAsync(parameter.CustomerId);
                if (member != null && member is Contact contact)
                {
                    parameter.Customer = _mapper.Map<Customer>(contact);
                }
            }
            await next(parameter);
        }

        private async Task InnerSetShopperDataFromMember(EvaluationContextBase evalContextBase, string customerId)
        {
            var member = await _memberIdResolver.ResolveMemberByIdAsync(customerId);
            if (member != null && member is Contact contact)
            {
                evalContextBase.ShopperGender = contact.GetDynamicPropertyValue("gender", string.Empty);
                if (contact.BirthDate != null)
                {
                    var zeroTime = new DateTime(1, 1, 1);
                    var span = DateTime.UtcNow - contact.BirthDate.Value;
                    evalContextBase.ShopperAge = (zeroTime + span).Year - 1;
                }
                evalContextBase.UserGroups = contact.Groups?.ToArray();
                evalContextBase.GeoTimeZone = contact.TimeZone;
                //PT-5445: Set other fields from customer 
            }
        }
    }
}
