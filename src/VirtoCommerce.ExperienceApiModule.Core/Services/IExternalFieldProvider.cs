using System;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.Core.Services
{
    public interface IExternalFieldProvider
    {
        FieldType AddField<TSourceType, TGraphType>(
            string typeName,
            string name,
            string description = null,
            QueryArguments arguments = null,
            Func<IResolveFieldContext<TSourceType>, object> resolve = null,
            string deprecationReason = null)
            where TGraphType : IGraphType;

        FieldType AddFieldAsync<TSourceType, TGraphType>(
            string typeName,
            string name,
            string description = null,
            QueryArguments arguments = null,
            Func<IResolveFieldContext<TSourceType>, Task<object>> resolve = null,
            string deprecationReason = null)
            where TGraphType : IGraphType;
    }
}
