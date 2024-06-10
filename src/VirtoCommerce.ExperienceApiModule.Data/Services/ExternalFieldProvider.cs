using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;
using VirtoCommerce.ExperienceApiModule.Core.Services;

namespace VirtoCommerce.ExperienceApiModule.Data.Services
{
    /// <summary>
    /// helps field merging
    /// need to be register as singleton
    /// </summary>
    public class ExternalFieldProvider : IExternalFieldProvider
    {
        public IDictionary<string, IList<FieldType>> Fields = new Dictionary<string, IList<FieldType>>();

        public FieldType AddField<TSourceType, TGraphType>(
            string typeName,
            string name,
            string description = null,
            QueryArguments arguments = null,
            Func<IResolveFieldContext<TSourceType>, object> resolve = null,
            string deprecationReason = null)
            where TGraphType : IGraphType
        {
            if (!Fields.TryGetValue(typeName, out var fields))
            {
                fields = new List<FieldType>();
                Fields.Add(typeName, fields);
            }

            var field = new FieldType
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

            fields.Add(field);
            return field;
        }

        public FieldType AddFieldAsync<TSourceType, TGraphType>(
            string typeName,
            string name,
            string description = null,
            QueryArguments arguments = null,
            Func<IResolveFieldContext<TSourceType>, Task<object>> resolve = null,
            string deprecationReason = null)
            where TGraphType : IGraphType
        {
            if (!Fields.TryGetValue(typeName, out var fields))
            {
                fields = new List<FieldType>();
                Fields.Add(typeName, fields);
            }

            var field = new FieldType
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

            fields.Add(field);
            return field;
        }
    }
}
