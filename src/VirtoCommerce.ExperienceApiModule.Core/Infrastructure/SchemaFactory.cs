using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL;
using GraphQL.Conversion;
using GraphQL.Instrumentation;
using GraphQL.Introspection;
using GraphQL.Types;
using GraphQL.Utilities;
using VirtoCommerce.ExperienceApiModule.Core.Schemas.ScalarTypes;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure
{
    public class SchemaFactory : ISchemaFactory, ISchema
    {
        private readonly IEnumerable<ISchemaBuilder> _schemaBuilders;
        private readonly IServiceProvider _services;
        private readonly ISchemaFilter _schemaFilter;

        private readonly Lazy<ISchema> _schema;

        public SchemaFactory(IEnumerable<ISchemaBuilder> schemaBuilders, IServiceProvider services, ISchemaFilter schemaFilter)
        {
            _schemaBuilders = schemaBuilders;
            _services = services;
            _schemaFilter = schemaFilter;

            _schema = new Lazy<ISchema>(GetSchema());
        }

        public bool Initialized => _schema.Value?.Initialized == true;
        public string Description { get; set; }

        public IObjectGraphType Query { get => _schema.Value?.Query; set => _schema.Value.Query = value; }
        public INameConverter NameConverter { get => _schema.Value?.NameConverter; }
        public IObjectGraphType Mutation { get => _schema.Value?.Mutation; set => _schema.Value.Mutation = value; }
        public IObjectGraphType Subscription { get => _schema.Value?.Subscription; set => _schema.Value.Subscription = value; }
        public SchemaDirectives Directives { get => _schema.Value?.Directives; }

        public SchemaTypes AllTypes => _schema.Value?.AllTypes;

        public IEnumerable<Type> AdditionalTypes => _schema.Value?.AdditionalTypes;

        public ISchemaFilter Filter { get => _schema.Value?.Filter; set => _schema.Value.Filter = value; }
        public FieldType SchemaMetaFieldType => _schema.Value?.SchemaMetaFieldType;
        public FieldType TypeMetaFieldType => _schema.Value?.TypeMetaFieldType;
        public FieldType TypeNameMetaFieldType => _schema.Value?.TypeNameMetaFieldType;

        public ExperimentalFeatures Features { get => _schema.Value?.Features; set => _schema.Value.Features = value; }

        public IFieldMiddlewareBuilder FieldMiddleware => _schema.Value?.FieldMiddleware;

        public IEnumerable<IGraphType> AdditionalTypeInstances => _schema.Value?.AdditionalTypeInstances;

        public IEnumerable<(Type clrType, Type graphType)> TypeMappings => _schema.Value?.TypeMappings;

        public IEnumerable<(Type clrType, Type graphType)> BuiltInTypeMappings => _schema.Value?.BuiltInTypeMappings;

        public ISchemaComparer Comparer { get => _schema.Value?.Comparer; set => _schema.Value.Comparer = value; }

        public Dictionary<string, object> Metadata => new Dictionary<string, object>();

        public ISchema GetSchema()
        {
            var schema = new Schema(_services)
            {
                Query = new ObjectGraphType { Name = "Query" },
                Mutation = new ObjectGraphType { Name = "Mutations" },
                Subscription = new ObjectGraphType { Name = "Subscriptions" },
                Filter = _schemaFilter,
            };

            foreach (var builder in _schemaBuilders)
            {
                builder.Build(schema);
            }

            // Map custom optional Graph Types for partial mutations support
            schema.RegisterTypeMapping<Optional<string>, OptionalStringGraphType>();
            schema.RegisterTypeMapping<Optional<decimal>, OptionalDecimalGraphType>();
            schema.RegisterTypeMapping<Optional<decimal?>, OptionalNullableDecimalGraphType>();
            schema.RegisterTypeMapping<Optional<int>, OptionalIntGraphType>();
            schema.RegisterTypeMapping<Optional<int?>, OptionalNullableIntGraphType>();

            // Map common value conversions to Optional
            ValueConverter.Register<int, Optional<int>>(x => new Optional<int>(x));
            ValueConverter.Register<decimal, Optional<decimal>>(x => new Optional<decimal>(x));
            ValueConverter.Register<string, Optional<string>>(x => new Optional<string>(x));

            // Clean Query, Mutation and Subscription if they have no fields
            // to prevent GraphQL configuration errors.

            if (!schema.Query.Fields.Any())
            {
                schema.Query = null;
            }

            if (!schema.Mutation.Fields.Any())
            {
                schema.Mutation = null;
            }

            return schema;
        }

        public void Initialize()
        {
            _schema.Value.Initialize();
        }

        public void RegisterType(IGraphType type)
        {
            _schema.Value.RegisterType(type);
        }

        public void RegisterType<T>() where T : IGraphType
        {
            _schema.Value.RegisterType<T>();
        }

        public void RegisterTypes(params IGraphType[] types)
        {
            _schema.Value.RegisterTypes(types);
        }

        public void RegisterTypes(params Type[] types)
        {
            _schema.Value.RegisterTypes(types);
        }

        public TType GetMetadata<TType>(string key, TType defaultValue = default)
        {
            return Metadata.ContainsKey(key) ? (TType)Metadata[key] : defaultValue;
        }

        public TType GetMetadata<TType>(string key, Func<TType> defaultValueFactory)
        {
            return Metadata.ContainsKey(key) ? (TType)Metadata[key] : defaultValueFactory();
        }

        public bool HasMetadata(string key)
        {
            return Metadata.ContainsKey(key);
        }

        public void RegisterVisitor(ISchemaNodeVisitor visitor)
        {
            _schema.Value?.RegisterVisitor(visitor);
        }

        public void RegisterVisitor(Type type)
        {
            _schema.Value?.RegisterVisitor(type);
        }

        public void RegisterType(Type type)
        {
            _schema.Value?.RegisterType(type);
        }

        public void RegisterTypeMapping(Type clrType, Type graphType)
        {
            _schema.Value?.RegisterTypeMapping(clrType, graphType);
        }
    }
}
