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

        /// <summary>
        /// Throws generic "Access denied" error
        /// </summary>
        public static void ThrowAccessDeniedError()
        {
            throw new AuthorizationError($"Access denied");
        }

        /// <summary>
        /// Throws "Password expired" error
        /// </summary>
        public static void ThrowPasswordExpiredError()
        {
            throw new AuthorizationError($"This user has their password expired. Please change the password using 'changePassword' command.");
        }

        /// <summary>
        /// Throws "User locked" error
        /// </summary>
        public static void ThrowUserLockedError()
        {
            throw new AuthorizationError($"This user is locked.");
        }
    }
}
