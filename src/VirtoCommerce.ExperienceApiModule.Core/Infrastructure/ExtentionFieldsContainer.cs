using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure
{
    /// <summary>
    /// helps field merging
    /// need to register as singleton
    /// </summary>
    public class ExtentionFieldsContainer
    {
        public IDictionary<string, IList<FieldType>> Fields = new Dictionary<string, IList<FieldType>>();

        public FieldType CreateField<TSourceType, TGraphType>(
            string name,
            string description = null,
            QueryArguments arguments = null,
            Func<IResolveFieldContext<TSourceType>, object> resolve = null,
            string deprecationReason = null)
            where TGraphType : IGraphType
        {
            return new FieldType
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
            };
        }

        public FieldType CreateFieldAsync<TSourceType, TGraphType>(
            string name,
            string description = null,
            QueryArguments arguments = null,
            Func<IResolveFieldContext<TSourceType>, Task<object>> resolve = null,
            string deprecationReason = null)
            where TGraphType : IGraphType
        {
            return new FieldType
            {
                Name = name,
                Description = description,
                DeprecationReason = deprecationReason,
                Type = GraphTypeExtenstionHelper.GetActualComplexType<TGraphType>(),
                Arguments = arguments,
                Resolver = resolve != null
                    ? new AsyncFieldResolver<TSourceType, object>(context =>
                    {
                        context.CopyArgumentsToUserContext();
                        return resolve(context);
                    })
                    : null
            };
        }
    }
}
