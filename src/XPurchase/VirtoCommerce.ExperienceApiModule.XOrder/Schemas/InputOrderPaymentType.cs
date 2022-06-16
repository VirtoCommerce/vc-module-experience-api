using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.ExperienceApiModule.XOrder.Models;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class InputOrderPaymentType : InputObjectGraphType<ExpOrderPayment>
    {
        public InputOrderPaymentType()
        {
            Field(x => x.Id, nullable: true).Description("Payment ID");
            Field(x => x.OuterId, nullable: true).Description("Payment outer ID value");
            Field(x => x.PaymentGatewayCode, nullable: true).Description("Payment gateway code value");
            Field(x => x.Currency, nullable: true);
            Field(x => x.Price, nullable: true);
            Field(x => x.Amount, nullable: true);

            Field<InputOrderAddressType>("billingAddress");
            Field<ListGraphType<InputDynamicPropertyValueType>>("dynamicProperties", "Dynamic properties");
        }
    }
}
