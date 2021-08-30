using GraphQL.Types;
using VirtoCommerce.PaymentModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class CorePaymentMethodType : ObjectGraphType<PaymentMethod>
    {
        public CorePaymentMethodType()
        {
            Field(x => x.Id, nullable: false);
            Field(x => x.Code, nullable: false);
            Field(x => x.LogoUrl, nullable: true);
        }
    }
}
