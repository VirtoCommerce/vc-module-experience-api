using System.Collections.Generic;
using System.Security.Claims;
using GraphQL.Authorization;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure
{
    public class GraphQLUserContext : Dictionary<string, object>, IProvideClaimsPrincipal
    {
        public GraphQLUserContext(ClaimsPrincipal user)
        {
            User = user;
        }
        public ClaimsPrincipal User { get; private set; }
    }
}
