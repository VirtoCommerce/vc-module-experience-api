using GraphQL;
using GraphQL.Types;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.ExperienceApiModule.XOrder.Extensions;
using VirtoCommerce.OrdersModule.Core.Model;

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
        
            Field<OrderMoneyType>(nameof(PaymentGatewayTransaction.Amount).ToCamelCase(), resolve: context => new Money(context.Source.Amount, context.GetCurrencyByCode(context.Source.CurrencyCode)));
          
        }
    }
}
