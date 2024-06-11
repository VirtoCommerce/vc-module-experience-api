using GraphQL.Types;
using VirtoCommerce.XOrder.Core.Models;

namespace VirtoCommerce.XOrder.Core.Schemas
{
    public class AuthorizePaymentResultType : ObjectGraphType<AuthorizePaymentResult>
    {
        public AuthorizePaymentResultType()
        {
            Field(x => x.IsSuccess);
            Field(x => x.ErrorMessage, nullable: true);
        }
    }
}
