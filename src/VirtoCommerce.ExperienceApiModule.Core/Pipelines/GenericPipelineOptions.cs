using System;
using System.Collections.Generic;

namespace VirtoCommerce.ExperienceApiModule.Core.Pipelines
{
#pragma warning disable S2326 // Unused type parameters should be removed
    public class GenericPipelineOptions<TParameter>
#pragma warning restore S2326 // Unused type parameters should be removed
    {
        public IList<Type> Middlewares { get; } = new List<Type>();
    }
}
