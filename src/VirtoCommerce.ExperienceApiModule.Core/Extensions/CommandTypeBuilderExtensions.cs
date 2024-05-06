using System.Linq;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.Core.Extensions
{
    public static class CommandTypeBuilderExtensions
    {
        /// <summary>
        /// Gets command builder for an existing query or command
        /// </summary>
        public static ICommandTypeBuilder UseCommandType<TCommandType>(this IServiceCollection services)
        {
            return new CommandTypeBuilder(services, typeof(TCommandType));
        }

        /// <summary>
        /// Overrides an existing query in abstracts factory and returns a new builder
        /// </summary>
        public static ICommandTypeBuilder OverrideQueryType<TQueryType, TExtendedQueryType>(this IServiceCollection services)
             where TExtendedQueryType : TQueryType
        {
            return services.OverrideCommandType<TQueryType, TExtendedQueryType>();
        }

        /// <summary>
        /// Overrides an existing  set of arguments in abstracts factory and returns a new builder
        /// </summary>
        public static ICommandTypeBuilder OverrideArgumentType<TArgumentType, TExtendedArgumentType>(this IServiceCollection services)
             where TExtendedArgumentType : TArgumentType
        {
            return services.OverrideCommandType<TArgumentType, TExtendedArgumentType>();
        }

        /// <summary>
        /// Overrides an existing  query or command in abstracts factory and returns a new builder
        /// </summary>
        public static ICommandTypeBuilder OverrideCommandType<TCommandType, TExtendedCommandType>(this IServiceCollection services)
             where TExtendedCommandType : TCommandType
        {
            AbstractTypeFactory<TCommandType>.OverrideType<TCommandType, TExtendedCommandType>();

            return new CommandTypeBuilder(services, typeof(TExtendedCommandType));
        }
        public static ICommandTypeBuilder WithQueryHandler<TExtendedQueryHandler>(this ICommandTypeBuilder builder)
        {
            return builder.WithCommandHandler<TExtendedQueryHandler>();
        }
        /// <summary>
        /// Registers a new handler for a query or command
        /// </summary>
        public static ICommandTypeBuilder WithCommandHandler<TExtendedCommandHandler>(this ICommandTypeBuilder builder)
        {
            if (builder?.CommandType == null)
            {
                return builder;
            }

            // construct generic request handler interface type
            var extendedHandlerType = typeof(TExtendedCommandHandler);
            var requestInterfaceTypes = extendedHandlerType.GetInterfaces().Where(x => x.Name.Contains("IRequestHandler") && x.IsGenericType && x.GenericTypeArguments.Length == 2);
            foreach (var requestInterfaceType in requestInterfaceTypes)
            {
                var requestType = requestInterfaceType.GenericTypeArguments.First();

                if (builder.CommandType.IsAssignableTo(requestType))
                {
                    var resultType = requestInterfaceType.GenericTypeArguments.Last();

                    var serviceType = typeof(IRequestHandler<,>).MakeGenericType(builder.CommandType, resultType);

                    builder.Services.AddTransient(serviceType, extendedHandlerType);
                }
            }

            return builder;
        }
    }
}
