using System;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure.Internal
{
    internal sealed class SchemaTypeBuilder<TSchemaType> : ISchemaTypeBuilder<TSchemaType> where TSchemaType : IGraphType
    {
        public SchemaTypeBuilder(IServiceCollection services)
        {
            Services = services;
            SchemaType = typeof(TSchemaType);
        }

        public IServiceCollection Services { get; }
        public Type SchemaType { get; }
        public Type UnderlyingType { get; }
    }
}
