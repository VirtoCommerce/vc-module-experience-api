using GraphQL;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.ExperienceApiModule.XOrder.Extensions;
using VirtoCommerce.OrdersModule.Core.Model;
using Money = VirtoCommerce.CoreModule.Core.Currency.Money;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class PaymentTransactionType : ObjectGraphType<PaymentGatewayTransaction>
    {
        public PaymentTransactionType()
        {
            Field(x => x.Id, nullable: false);
            Field(x => x.IsProcessed, nullable: false);
            Field(x => x.ProcessedDate, nullable: true);
            Field(x => x.ProcessError, nullable: true);
            Field(x => x.ProcessAttemptCount, nullable: false);
            Field(x => x.RequestData, nullable: true);
            Field(x => x.ResponseData, nullable: true);
            Field(x => x.ResponseCode, nullable: true);
            Field(x => x.GatewayIpAddress, nullable: true);
            Field(x => x.Type, nullable: true);
            Field(x => x.Status, nullable: true);
            Field(x => x.Note, nullable: true);
        
            Field<NonNullGraphType<MoneyType>>(nameof(PaymentGatewayTransaction.Amount).ToCamelCase(), resolve: context => new Money(context.Source.Amount, context.GetOrderCurrencyByCode(context.Source.CurrencyCode)));
          
        }
    }
}
