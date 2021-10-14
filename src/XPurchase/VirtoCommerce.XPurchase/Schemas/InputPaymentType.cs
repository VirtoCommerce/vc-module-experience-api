using GraphQL.Types;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputPaymentType : InputObjectGraphType<PaymentOptional>
    {
        public InputPaymentType()
        {
            Field(x => x.Id, nullable: true).Description("the payment id");
            Field(x => x.OuterId, nullable: true).Description("Value of payment outer id");
            Field(x => x.PaymentGatewayCode, nullable: true).Description("Value of payment gateway code");
            Field<InputAddressType>("billingAddress");
            Field(x => x.Currency, nullable: true);
            Field(x => x.Price, nullable: true);
            Field(x => x.Amount, nullable: true);
        }
    }
}
