using GraphQL.Types;
using VirtoCommerce.PaymentModule.Model.Requests;

namespace VirtoCommerce.XOrder.Core.Schemas
{
    public class ProcessPaymentRequestResultType : ObjectGraphType<ProcessPaymentRequestResult>
    {
        public ProcessPaymentRequestResultType()
        {
            Field(x => x.IsSuccess);
            Field(x => x.HtmlForm, nullable: true);
            Field<StringGraphType>("newPaymentStatus",
                "New payment status",
                resolve: context => context.Source.NewPaymentStatus.ToString());
            Field(x => x.OuterId, nullable: true);
            Field(x => x.RedirectUrl, nullable: true);
            Field(x => x.ErrorMessage, nullable: true);
        }
    }
}
