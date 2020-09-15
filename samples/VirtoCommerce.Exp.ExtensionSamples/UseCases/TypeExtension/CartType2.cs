using GraphQL.Types;
using VirtoCommerce.XPurchase.Schemas;
using VirtoCommerce.XPurchase.Services;

namespace VirtoCommerce.Exp.ExtensionSamples.UseCases.TypeExtension
{
    public class CartType2 : CartType
    {
        public CartType2(ICartAvailMethodsService cartAvailMethods) : base(cartAvailMethods)
        {
            Field<StringGraphType>("myCoolProperty", resolve: context => "my cool value" );
        }
    }
}
