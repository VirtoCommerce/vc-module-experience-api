using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using PipelineNet.Middleware;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.PricingModule.Core.Model;
using VirtoCommerce.TaxModule.Core.Model;

namespace VirtoCommerce.XProfile.Middlewares
{
    public class ProfileEvalContextBuildMiddleware : IAsyncMiddleware<PromotionEvaluationContext>, IAsyncMiddleware<PriceEvaluationContext>, IAsyncMiddleware<TaxEvaluationContext>
    {
        private readonly IMapper _mapper;
        private readonly IMemberService _memberService;
        private readonly UserManager<ApplicationUser> _userManager;
        public ProfileEvalContextBuildMiddleware(
            IMapper mapper
            , IMemberService memberService
            , Func<UserManager<ApplicationUser>> userManager)
        {
            _mapper = mapper;
            _memberService = memberService;
            _userManager = userManager();
        }

        public async Task Run(PromotionEvaluationContext parameter, Func<PromotionEvaluationContext, Task> next)
        {
            await InnerSetShopperDataFromMember(parameter, parameter.CustomerId);
            await next(parameter);
        }

        public async Task Run(PriceEvaluationContext parameter, Func<PriceEvaluationContext, Task> next)
        {
            await InnerSetShopperDataFromMember(parameter, parameter.CustomerId);
            await next(parameter);
        }

        public async Task Run(TaxEvaluationContext parameter, Func<TaxEvaluationContext, Task> next)
        {
            var contact = await FindContactByUserOrMemberId(parameter.CustomerId);
            if (contact != null)
            {
                parameter.Customer = _mapper.Map<Customer>(contact);
            }
            await next(parameter);
        }

        private async Task InnerSetShopperDataFromMember(EvaluationContextBase evalContextBase, string customerId)
        {
            var contact = await FindContactByUserOrMemberId(customerId);


            if(contact != null)
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
                //TODO: Set other fields from customer 
            }
        }

        //TODO: DRY violation in many places in this solution. Move to abstraction to from multiple boundaries
        protected virtual async Task<Contact> FindContactByUserOrMemberId(string id)
        {
            // Try to find contact
            var result = await _memberService.GetByIdAsync(id);

            if (result == null)
            {
                var user = await _userManager.FindByIdAsync(id);

                if (user != null && user.MemberId != null)
                {
                    result = await _memberService.GetByIdAsync(user.MemberId);
                }
            }

            return result as Contact;
        }
    }
}
