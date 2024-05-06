using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GraphQL.Authorization;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.Platform.Security.Authorization;
using AuthorizationResult = GraphQL.Authorization.AuthorizationResult;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure.Authorization
{
    public class PermissionAuthorizationEvaluator : GraphQL.Authorization.IAuthorizationEvaluator
    {
        private readonly IAuthorizationService _authorizationService;

        public PermissionAuthorizationEvaluator(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public async Task<AuthorizationResult> Evaluate(ClaimsPrincipal principal, IDictionary<string, object> userContext, IReadOnlyDictionary<string, object> inputs, IEnumerable<string> requiredPolicies)
        {
            var context = new AuthorizationContext
            {
                User = principal ?? new ClaimsPrincipal(new ClaimsIdentity()),
                UserContext = userContext,
                Inputs = inputs,
            };

            foreach (var requiredPolicy in requiredPolicies?.ToList() ?? new List<string>())
            {
                var authorizationResult = await _authorizationService.AuthorizeAsync(context.User, null, new PermissionAuthorizationRequirement(requiredPolicy));
                if (!authorizationResult.Succeeded)
                {
                    context.ReportError($"User doesn't have the required permission '{requiredPolicy}'.");
                }
            }

            return !context.HasErrors ? AuthorizationResult.Success() : AuthorizationResult.Fail(context.Errors);
        }
    }
}
