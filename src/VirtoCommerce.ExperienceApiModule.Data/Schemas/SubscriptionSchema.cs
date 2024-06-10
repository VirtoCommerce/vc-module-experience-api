using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Subscription;
using GraphQL.Types;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure.Authorization;
using VirtoCommerce.Platform.Security.Authorization;

namespace VirtoCommerce.ExperienceApiModule.Data.Schemas
{
    public class SubscriptionSchema(IAuthorizationService authorizationService) : ISchemaBuilder
    {
        public void Build(ISchema schema)
        {
            var pingType = new EventStreamFieldType
            {
                Name = "ping",
                Type = typeof(StringGraphType),
                Resolver = new FuncFieldResolver<string>(Resolve),
                AsyncSubscriber = new AsyncEventStreamResolver<string>(Subscribe),
            };
            schema.Subscription.AddField(pingType);
        }

        private string Resolve(IResolveFieldContext context)
        {
            return context.Source as string;
        }

        private async Task<IObservable<string>> Subscribe(IResolveEventStreamContext context)
        {
            var authorizationResult = await authorizationService.AuthorizeAsync(context.GetCurrentPrincipal(), null, new PermissionAuthorizationRequirement(string.Empty));
            if (!authorizationResult.Succeeded)
            {
                throw AuthorizationError.Forbidden();
            }

            // reserved for future use
            return Observable.Never<string>();
        }
    }
}
