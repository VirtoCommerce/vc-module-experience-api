using System;
using System.Collections;
using GraphQL;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure.Authorization
{
    /// <summary>
    /// Represents an auhorization error
    /// </summary>
    public class AuthorizationError : ExecutionError
    {
        public AuthorizationError(string message) : base(message) => Code = Constants.UnauthorizedCode;

        public AuthorizationError(string message, IDictionary data) : base(message, data) => Code = Constants.UnauthorizedCode;

        public AuthorizationError(string message, Exception exception) : base(message, exception) => Code = Constants.UnauthorizedCode;
    }
}
