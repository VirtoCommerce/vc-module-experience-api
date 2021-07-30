using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries
{
    public class CheckEmailUniquenessResult : ObjectGraphType<CheckEmailUniquenessResponse>
    {
        public CheckEmailUniquenessResult()
        {
            Field(x => x.IsUnique).Description("Email uniqueness flag");
        }
    }
}
