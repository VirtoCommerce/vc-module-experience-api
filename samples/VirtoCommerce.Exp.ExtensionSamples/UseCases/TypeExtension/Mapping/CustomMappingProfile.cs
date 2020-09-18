using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.XPurchase;

namespace VirtoCommerce.Exp.ExtensionSamples.UseCases.TypeExtension.Mapping
{
    public class CustomMappingProfile : Profile
    {
        public CustomMappingProfile()
        {
            //CreateMap<CartAggregate, PromotionEvaluationContext>().AfterMap((lineItem, promoEvalcontext, context) =>
            //{
            //    promoEvalcontext.GeoCity = "cool";
            //});
        }
    }
}
