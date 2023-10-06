using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class PaymentMethodTypeType: EnumerationGraphType<PaymentModule.Core.Model.PaymentMethodType>
    {
        public PaymentMethodTypeType()
        {
            Name = nameof(PaymentMethodTypeType);
        }
    }
}
