using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.XOrder.Models;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
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
