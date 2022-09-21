using System.Collections.Generic;
using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

public interface IHasArguments
{
    IEnumerable<QueryArgument> GetArguments();
}
