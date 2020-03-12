using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Data.GraphQL.Schemas;
using Xunit;

namespace VirtoCommerce.ExportModule.Tests
{
    public class CsvExportProviderTests
    {
        public class Foo
        {
            public string Name { get; set; }
        }
        public class FooType : ObjectGraphType<Foo>
        {
            public FooType()
            {
                Name = "Foo";
                Field(x => x.Name);
                //This nested field cause 'A loop has been detected while registering schema types.' exception in GraphTypesLookup.AddTypeWithLoopCheck
                Field<FooType>("parent", resolve: (ctx) => new Foo());
            }
        }
        public class FooType2 : FooType
        {
        }
        public class RootQuery : ObjectGraphType<object>
        {
            public RootQuery()
            {
                Field<FooType>(
                    "foo",
                    resolve: context => new FooType() { }
                );
            }
        }
        public class MySchema : Schema
        {
            public MySchema(IServiceProvider provider)
                : base(provider)
            {
                Query = new RootQuery();
            }
        }

        [Fact]
        public Task throw_loop_has_been_detected_for_use_derived_type()
        {
            var services = new SimpleContainer();
            //Create a derived type FooType2 for FooType in type resolving
            services.Register<FooType>(() => new FooType2());
            var schema = new MySchema(new SimpleContainerAdapter(services));
            
            schema.Initialize(); //throw the  'A loop has been detected while registering schema types.' exception

            return Task.CompletedTask;
        }

        
    }

    public class SimpleContainerAdapter : IServiceProvider
    {
        private readonly ISimpleContainer _container;

        public SimpleContainerAdapter(ISimpleContainer container)
        {
            _container = container;
        }

        public object GetService(Type serviceType)
        {
            return _container.Get(serviceType);
        }
    }

    public interface ISimpleContainer : IDisposable
    {
        object Get(Type serviceType);
        T Get<T>();
        void Register<TService>();
        void Register<TService>(Func<TService> instanceCreator);
        void Register<TService, TImpl>() where TImpl : TService;
        void Singleton<TService>(TService instance);
        void Singleton<TService>(Func<TService> instanceCreator);
    }

    public class SimpleContainer : ISimpleContainer
    {
        private readonly Dictionary<Type, Func<object>> _registrations = new Dictionary<Type, Func<object>>();

        public void Register<TService>()
        {
            Register<TService, TService>();
        }

        public void Register<TService, TImpl>() where TImpl : TService
        {
            _registrations.Add(typeof(TService),
                () =>
                {
                    var implType = typeof(TImpl);
                    return typeof(TService) == implType
                        ? CreateInstance(implType)
                        : Get(implType);
                });
        }

        public void Register<TService>(Func<TService> instanceCreator)
        {
            _registrations.Add(typeof(TService), () => instanceCreator());
        }

        public void Singleton<TService>(TService instance)
        {
            _registrations.Add(typeof(TService), () => instance);
        }

        public void Singleton<TService>(Func<TService> instanceCreator)
        {
            var lazy = new Lazy<TService>(instanceCreator);
            Register(() => lazy.Value);
        }

        public T Get<T>()
        {
            return (T)Get(typeof(T));
        }

        public object Get(Type serviceType)
        {
            if (_registrations.TryGetValue(serviceType, out var creator))
            {
                return creator();
            }

            if (!serviceType.IsAbstract)
            {
                return CreateInstance(serviceType);
            }

            throw new InvalidOperationException("No registration for " + serviceType);
        }

        public void Dispose()
        {
            _registrations.Clear();
        }

        private object CreateInstance(Type implementationType)
        {
            var ctor = implementationType.GetConstructors().OrderByDescending(x => x.GetParameters().Length).First();
            var parameterTypes = ctor.GetParameters().Select(p => p.ParameterType);
            var dependencies = parameterTypes.Select(Get).ToArray();
            return Activator.CreateInstance(implementationType, dependencies);
        }
    }
}
