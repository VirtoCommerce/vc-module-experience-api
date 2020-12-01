using GraphQL.Types;
using VirtoCommerce.CartModule.Core.Model;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputPaymentType : InputObjectGraphType<Payment>
    {
        public InputPaymentType()
        {
            Field(x => x.Id, nullable: true).Description("the payment id");
            Field(x => x.OuterId, nullable: true).Description("Value of payment outer id");
            Field(x => x.PaymentGatewayCode, nullable: false).Description("Value of payment gateway code");
            Field<InputAddressType>("billingAddress");
            Field(x => x.Currency, nullable: true);
            Field(x => x.Price, nullable: true);
            Field(x => x.Amount, nullable: true);
        }
    }
}
