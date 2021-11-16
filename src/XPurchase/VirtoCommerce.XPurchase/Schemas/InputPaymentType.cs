using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputPaymentType : InputObjectGraphType<ExpCartPayment>
    {
        public InputPaymentType()
        {
            Field(x => x.Id, nullable: true).Description("The payment Id");
            Field(x => x.OuterId, nullable: true).Description("Value of payment outer Id");
            Field(x => x.PaymentGatewayCode, nullable: true).Description("Value of payment gateway code");
            Field<InputAddressType>("billingAddress");
            Field(x => x.Currency, nullable: true);
            Field(x => x.Price, nullable: true);
            Field(x => x.Amount, nullable: true);

            Field<ListGraphType<InputDynamicPropertyValueType>>("dynamicProperties",
                "Dynamic properties");
        }
    }
}
