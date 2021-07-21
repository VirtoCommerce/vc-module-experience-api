using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure.Authorization;

namespace VirtoCommerce.ExperienceApiModule.Core.Middleware
{
    public class AuthorizationErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizationErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (AuthorizationException ex) when (context.Request.Path.ToString().Contains("graphql"))
            {
                var json = JsonConvert.SerializeObject(new { error = ex.Message });
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync(json);
            }
        }
    }
}
