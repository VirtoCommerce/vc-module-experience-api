using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Services;
using VirtoCommerce.XPurchase.Schemas;
using VirtoCommerce.XPurchase.Services;

namespace VirtoCommerce.Exp.ExtensionSamples
{
    public class CartType2 : CartType
    {
        public CartType2(ICartAvailMethodsService cartAvailMethods, IDynamicPropertyResolverService dynamicPropertyResolver)
            : base(cartAvailMethods, dynamicPropertyResolver, null)
        {
            Name = "CartType";

            Field<StringGraphType>("myCoolScalarProperty", resolve: context => "my cool value");
        }
    }
}
