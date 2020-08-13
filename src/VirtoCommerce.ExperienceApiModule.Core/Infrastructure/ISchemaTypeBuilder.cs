using System;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure
{
    /// <summary>
    /// GraphQL schema type  builder used for declaration of new type extension via fluent syntax and specific extension methods as 'this' argument.
    /// </summary>
    public interface ISchemaTypeBuilder<TSchemaType> where TSchemaType : IGraphType
    {
        Type SchemaType { get; }
        IServiceCollection Services { get; }
    }
}
