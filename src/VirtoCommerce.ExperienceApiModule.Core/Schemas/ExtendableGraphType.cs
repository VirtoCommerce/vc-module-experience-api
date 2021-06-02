using System;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class ExtendableGraphType<TSourceType> : ObjectGraphType<TSourceType>
    {
        public FieldType ExtendableField<TGraphType>(
            string name,
            string description = null,
            QueryArguments arguments = null,
            Func<IResolveFieldContext<TSourceType>, object> resolve = null,
            string deprecationReason = null)
            where TGraphType : IGraphType
        {
            return AddField(new FieldType
            {
                Name = name,
                Description = description,
                DeprecationReason = deprecationReason,
                Type = GraphTypeExtenstionHelper.GetActualComplexType<TGraphType>(),
                Arguments = arguments,
                Resolver = resolve != null
                    ? new FuncFieldResolver<TSourceType, object>(context =>
                        {
                            context.CopyArgumentsToUserContext();
                            return resolve(context);
                        })
                    : null
            });
        }
    }
}
