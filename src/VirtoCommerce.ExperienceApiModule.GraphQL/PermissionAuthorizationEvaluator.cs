using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GraphQL.Authorization;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.Platform.Security.Authorization;

namespace VirtoCommerce.ExperienceApiModule.GraphQLEx
{
    public class PermissionAuthorizationEvaluator : GraphQL.Authorization.IAuthorizationEvaluator
    {
        private readonly IAuthorizationService _authorizationService;

        public PermissionAuthorizationEvaluator(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public async Task<GraphQL.Authorization.AuthorizationResult> Evaluate(
            ClaimsPrincipal principal,
            object userContext,
            Dictionary<string, object> inputVariables,
            IEnumerable<string> requiredPolicies)
        {
            var context = new AuthorizationContext
            {
                User = principal ?? new ClaimsPrincipal(new ClaimsIdentity()),
                UserContext = userContext,
                InputVariables = inputVariables
            };

            foreach (var requiredPolicy in requiredPolicies?.ToList())
            {
                var authorizationResult = await _authorizationService.AuthorizeAsync(context.User, null, new PermissionAuthorizationRequirement(requiredPolicy));
                if (!authorizationResult.Succeeded)
                {
                    context.ReportError($"User doesn't have the required permission '{requiredPolicy}'.");
                }
            }
            return !context.HasErrors ? GraphQL.Authorization.AuthorizationResult.Success() : GraphQL.Authorization.AuthorizationResult.Fail(context.Errors);
        }
    }
}
