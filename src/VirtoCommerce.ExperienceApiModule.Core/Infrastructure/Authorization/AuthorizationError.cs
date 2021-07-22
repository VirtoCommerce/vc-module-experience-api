using System;
using System.Collections;
using GraphQL;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure.Authorization
{
    /// <summary>
    /// Represents auhorization error
    /// Thrown in GraphQLMiddleware on auth errors.
    /// Catched and handled in AuthorizationErrorHandlingMiddleware to return 401 error code to client
    /// </summary>
    public class AuthorizationError : ExecutionError
    {
        public AuthorizationError(string message) : base(message)
        {
        }

        public AuthorizationError(string message, IDictionary data) : base(message, data)
        {
        }

        public AuthorizationError(string message, Exception exception) : base(message, exception)
        {
        }
    }
}
