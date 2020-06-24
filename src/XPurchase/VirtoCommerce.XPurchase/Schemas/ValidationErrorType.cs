using GraphQL.Types;
using VirtoCommerce.XPurchase.Domain.CartAggregate;

namespace VirtoCommerce.XPurchase.Schemas
{
    public class ValidationErrorType : ObjectGraphType<CartValidationError>
    {
        public ValidationErrorType()
        {
            Field(x => x.ErrorCode, nullable: true).Description("Error code");
            Field(x => x.Error, nullable: true).Description("Error msg");
            Field(x => x.ObjectId, nullable: true).Description("Object id");
            Field(x => x.ObjectType, nullable: true).Description("Object type");
        }
    }
}
