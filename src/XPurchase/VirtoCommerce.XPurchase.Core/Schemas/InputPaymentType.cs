using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.XPurchase.Core.Models;

namespace VirtoCommerce.XPurchase.Core.Schemas
{
    public class InputPaymentType : InputObjectGraphType<ExpCartPayment>
    {
        public InputPaymentType()
        {
            Field(x => x.Id, nullable: true).Description("Payment ID");
            Field(x => x.OuterId, nullable: true).Description("Payment outer ID value");
            Field(x => x.PaymentGatewayCode, nullable: true).Description("Payment gateway code value");
            Field<InputAddressType>("billingAddress");
            Field(x => x.Purpose, nullable: true);
            Field(x => x.Currency, nullable: true);
            Field(x => x.Price, nullable: true);
            Field(x => x.Amount, nullable: true);
            Field(x => x.VendorId, nullable: true);
            Field(x => x.Comment, nullable: true).Description("Text comment");

            Field<ListGraphType<InputDynamicPropertyValueType>>("dynamicProperties",
                "Dynamic properties");
        }
    }
}
