using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL.Conversion;
using GraphQL.Introspection;
using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.Core.Schema
{
    public class SchemaFactory : ISchemaFactory, ISchema
    {
        private readonly IEnumerable<ISchemaBuilder> _schemaBuilders;
        private readonly IServiceProvider _services;


        private readonly Lazy<ISchema> _schema;

        public SchemaFactory(IEnumerable<ISchemaBuilder> schemaBuilders, IServiceProvider services)
        {
            _schemaBuilders = schemaBuilders;
            _services = services;
            _schema = new Lazy<ISchema>(GetSchema());
        }

        public bool Initialized => _schema.Value?.Initialized == true;

        public IFieldNameConverter FieldNameConverter { get => _schema.Value?.FieldNameConverter; set => _schema.Value.FieldNameConverter = value; }
        public IObjectGraphType Query { get => _schema.Value?.Query; set => _schema.Value.Query = value; }
        public IObjectGraphType Mutation { get => _schema.Value?.Mutation; set => _schema.Value.Mutation = value; }
        public IObjectGraphType Subscription { get => _schema.Value?.Subscription; set => _schema.Value.Subscription = value; }
        public IEnumerable<DirectiveGraphType> Directives { get => _schema.Value?.Directives; set => _schema.Value.Directives = value; }

        public IEnumerable<IGraphType> AllTypes => _schema.Value?.AllTypes;

        public IEnumerable<Type> AdditionalTypes => _schema.Value?.AdditionalTypes;

        public ISchemaFilter Filter { get => _schema.Value?.Filter; set => _schema.Value.Filter = value; }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _schema.Value.Dispose();
            }
        }


        public DirectiveGraphType FindDirective(string name)
        {
            return _schema.Value?.FindDirective(name);
        }

        public IGraphType FindType(string name)
        {
            return _schema.Value?.FindType(name);
        }

        public IAstFromValueConverter FindValueConverter(object value, IGraphType type)
        {
            return _schema.Value?.FindValueConverter(value, type);
        }

        public ISchema GetSchema()
        {
            var schema = new GraphQL.Types.Schema(_services)
            {
                Query = new ObjectGraphType { Name = "Query" }
            };

            foreach (var builder in _schemaBuilders)
            {
                builder.Build(schema);
            }


            // Clean Query, Mutation and Subscription if they have no fields
            // to prevent GraphQL configuration errors.

            if (!schema.Query.Fields.Any())
            {
                schema.Query = null;
            }

            return schema;
        }

        public void Initialize()
        {
            _schema.Value.Initialize();
        }

        public void RegisterDirective(DirectiveGraphType directive)
        {
            _schema.Value.RegisterDirective(directive);
        }

        public void RegisterDirectives(params DirectiveGraphType[] directives)
        {
            _schema.Value.RegisterDirectives(directives);
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

        public void RegisterValueConverter(IAstFromValueConverter converter)
        {
            _schema.Value.RegisterValueConverter(converter);
        }
    }
}
