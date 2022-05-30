using GraphQL;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.ExperienceApiModule.XOrder.Models;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public class InitializePaymentResultType : ObjectGraphType<InitializePaymentResult>
    {
        public InitializePaymentResultType()
        {
            Field(x => x.IsSuccess);
            Field(x => x.ErrorMessage, nullable: true);
            Field(x => x.StoreId, nullable: true);
            Field(x => x.PaymentId, nullable: true);
            Field(x => x.OrderId, nullable: true);
            Field(x => x.OrderNumber, nullable: true);
            Field(x => x.PaymentMethodCode, nullable: true);
            Field(x => x.PaymentActionType, nullable: true);
            Field(x => x.ActionRedirectUrl, nullable: true);
            Field(x => x.ActionHtmlForm, nullable: true);
            Field<ListGraphType<KeyValueType>>(nameof(InitializePaymentResult.PublicParameters).ToCamelCase(), resolve: context => context.Source.PublicParameters);
        }
    }
}
