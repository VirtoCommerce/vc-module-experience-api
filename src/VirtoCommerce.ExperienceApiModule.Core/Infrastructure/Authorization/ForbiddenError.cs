using System;
using System.Collections;
using GraphQL;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure.Authorization
{
    [Obsolete("Use AuthorizationError", DiagnosticId = "VC0005", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
    public class ForbiddenError : ExecutionError
    {
        public ForbiddenError(string message) : base(message) => Code = Constants.ForbiddenCode;
        public ForbiddenError(string message, IDictionary data) : base(message, data) => Code = Constants.ForbiddenCode;
        public ForbiddenError(string message, Exception exception) : base(message, exception) => Code = Constants.ForbiddenCode;
    }
}
