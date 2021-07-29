using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries
{
    public class CheckUsernameUniquenessResult : ObjectGraphType<CheckUsernameUniquenessResponse>
    {
        public CheckUsernameUniquenessResult()
        {
            Field(x => x.IsUnique).Description("Username uniqueness flag");
        }
    }
}
