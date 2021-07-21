using System;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure.Authorization
{
    /// <summary>
    /// Represents authorization error. Catched and handled in AuthorizationErrorHandlingMiddleware to return 401 error code to client
    /// </summary>
    public class AuthorizationException : Exception
    {
        public AuthorizationException()
        {
        }

        public AuthorizationException(string message) : base(message)
        {
        }

        public AuthorizationException(string message, Exception exception) : base(message, exception)
        {
        }
    }
}
