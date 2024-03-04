using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GraphQL.Server.Transports.Subscriptions.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json.Linq;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions
{
    /// <summary>
    /// Try to resolver user context for subscriptions by validating JWT token in the request's Authorization header
    /// </summary>
    public class SubscriptionsUserContextResolver(
        IHttpContextAccessor httpContextAccessor,
        IOptionsSnapshot<JwtBearerOptions> jwtBearerOptions)
        : IOperationMessageListener
    {
        private const string ContextKey = "User";
        private const string AuthorizationHeader = "Authorization";

        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IOptionsSnapshot<JwtBearerOptions> _jwtBearerOptions = jwtBearerOptions;

        public async Task BeforeHandleAsync(MessageHandlingContext context)
        {
            if (context.Message.Type != MessageType.GQL_CONNECTION_INIT)
            {
                return;
            }

            if (context.Message.Payload is JObject payload && payload.ContainsKey(AuthorizationHeader))
            {
                var authorization = payload.Value<string>(AuthorizationHeader);

                // set the ClaimsPrincipal for the HttpContext; authentication will take place against this object
                //_httpContextAccessor.HttpContext.User = BuildClaimsPrincipal(authorization);
                var principal = await BuildClaimsPrincipal(authorization);
            }

            //context.Properties[ContextKey] = _httpContextAccessor.HttpContext.User;

            return;
        }

        private async Task<ClaimsPrincipal> BuildClaimsPrincipal(string authorization)
        {
            ClaimsPrincipal principal = null;

            if (!authorization.StartsWith("Bearer ", StringComparison.Ordinal))
            {
                return principal;
            }

            try
            {
                var token = authorization[7..];
                var tokenOptions = _jwtBearerOptions.Get(JwtBearerDefaults.AuthenticationScheme);

                var jsonWebTokenHandler = tokenOptions.TokenHandlers.OfType<JsonWebTokenHandler>().FirstOrDefault();
                if (jsonWebTokenHandler != null)
                {
                    var result = await jsonWebTokenHandler.ValidateTokenAsync(token, tokenOptions.TokenValidationParameters);
                    var claimsPrincipal = new ClaimsPrincipal(result.ClaimsIdentity);
                }
            }
            catch
            {
                // attempting to validate an invalid JWT token will result in an exception
                // ignore it and proceed as an anonymous user or already authenticated user
            }

            return principal;
        }

        public Task HandleAsync(MessageHandlingContext context) => Task.CompletedTask;

        public Task AfterHandleAsync(MessageHandlingContext context) => Task.CompletedTask;
    }
}
