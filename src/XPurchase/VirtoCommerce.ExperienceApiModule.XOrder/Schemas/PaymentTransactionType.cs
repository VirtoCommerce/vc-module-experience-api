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
            Field(x => x.Id);
            Field(x => x.IsProcessed, false);
            Field(x => x.ProcessedDate, true);
            Field(x => x.ProcessError, true);
            Field(x => x.ProcessAttemptCount);
            Field(x => x.RequestData);
            Field(x => x.ResponseData);
            Field(x => x.ResponseCode);
            Field(x => x.GatewayIpAddress);
            Field(x => x.Type);
            Field(x => x.Status);
            Field(x => x.Note);
        
            Field<MoneyType>(nameof(PaymentGatewayTransaction.Amount).ToCamelCase(), resolve: context => new Money(context.Source.Amount, context.GetOrderCurrencyByCode(context.Source.CurrencyCode)));
          
        }
    }
}
