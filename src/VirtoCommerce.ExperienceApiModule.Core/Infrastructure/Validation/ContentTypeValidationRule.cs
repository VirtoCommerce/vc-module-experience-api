using System.Threading.Tasks;
using GraphQL.Server.Transports.AspNetCore;
using GraphQL.Validation;
using Microsoft.AspNetCore.Http;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure.Validation
{
    public class ContentTypeValidationRule : IValidationRule
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ContentTypeValidationRule(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public virtual Task<INodeVisitor> ValidateAsync(ValidationContext context)
        {
            var contentType = _httpContextAccessor?.HttpContext?.Request?.ContentType;

            if (contentType == MediaType.JSON ||
                contentType == MediaType.GRAPH_QL ||
                (contentType == null && _httpContextAccessor.HttpContext.WebSockets?.IsWebSocketRequest == true))
            {
                return default;
            }

            context.ReportError(new ValidationError(string.Empty, string.Empty, "Non-supported media type."));
            return default;
        }
    }
}
