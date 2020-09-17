using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;

namespace VirtoCommerce.Exp.ExtensionSamples.UseCases.TypeExtension.Mapping
{
    public class CustomMappingProfile : Profile
    {
        public CustomMappingProfile()
        {
            CreateMap<LineItem, ProductPromoEntry>().ConvertUsing((lineItem, productPromoEntry, context) =>
            {
                productPromoEntry.Attributes["myCoolCustomAttribute"] = "cool";
                return productPromoEntry;
            });
        }
    }
}
