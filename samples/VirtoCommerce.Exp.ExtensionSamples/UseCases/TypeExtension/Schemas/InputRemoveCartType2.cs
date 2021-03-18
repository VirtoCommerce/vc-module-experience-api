using GraphQL.Types;
using VirtoCommerce.XPurchase.Schemas;

namespace VirtoCommerce.Exp.ExtensionSamples
{
    public class InputRemoveCartType2 : InputRemoveCartType
    {
        public InputRemoveCartType2()
        {
            Field<NonNullGraphType<StringGraphType>>("reason");
        }
    }
}
