using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class PaymentMethodGroupTypeType: EnumerationGraphType<PaymentModule.Core.Model.PaymentMethodGroupType>
    {
        public PaymentMethodGroupTypeType()
        {
            Name = nameof(PaymentMethodGroupTypeType);
        }
    }
}
