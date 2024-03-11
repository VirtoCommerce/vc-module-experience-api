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

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions.Infrastructure
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
            switch (context.Message.Type)
            {
                case MessageType.GQL_CONNECTION_INIT:
                    {
                        if (context.Message.Payload is JObject payload && payload.ContainsKey(AuthorizationHeader))
                        {
                            var authorization = payload.Value<string>(AuthorizationHeader);

                            // set the ClaimsPrincipal for the HttpContext; authentication will take place against this object
                            var principal = await BuildClaimsPrincipal(authorization);
                            _httpContextAccessor.HttpContext.User = principal;
                            context.TryAdd(ContextKey, _httpContextAccessor.HttpContext.User);
                        }

                        break;
                    }

                default:
                    context.TryAdd(ContextKey, _httpContextAccessor.HttpContext.User);
                    break;
            }

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
                    principal = new ClaimsPrincipal(result.ClaimsIdentity);
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
