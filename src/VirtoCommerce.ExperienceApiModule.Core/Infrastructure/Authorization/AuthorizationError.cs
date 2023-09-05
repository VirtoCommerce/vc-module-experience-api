using System;
using System.Collections;
using GraphQL;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure.Authorization
{
    /// <summary>
    /// Represents an authorization error
    /// </summary>
    public class AuthorizationError : ExecutionError
    {
        public AuthorizationError(string message) : base(message) => Code = Constants.UnauthorizedCode;

        public AuthorizationError(string message, IDictionary data) : base(message, data) => Code = Constants.UnauthorizedCode;

        public AuthorizationError(string message, Exception exception) : base(message, exception) => Code = Constants.UnauthorizedCode;

        public AuthorizationError(string message, string code) : base(message) => Code = code;

        /// <summary>
        /// Creates "Anonymous access denied" error
        /// </summary>
        public static AuthorizationError AnonymousAccessDenied()
        {
            return new AuthorizationError("Anonymous access denied or access token has expired or is invalid.", Constants.UnauthorizedCode);
        }

        /// <summary>
        /// Creates "Password expired" error
        /// </summary>
        public static AuthorizationError PasswordExpired()
        {
            return new AuthorizationError("This user has their password expired. Please change the password using 'changePassword' command.", Constants.PasswordExpiredCode);
        }

        /// <summary>
        /// Creates "User locked" error
        /// </summary>
        public static AuthorizationError UserLocked()
        {
            return new AuthorizationError("This user is locked.", Constants.UserLockedCode);
        }

        /// <summary>
        /// Creates generic "Access denied" error
        /// </summary>
        public static AuthorizationError Forbidden()
        {
            return new AuthorizationError("Access denied.") { Code = Constants.ForbiddenCode };
        }

        /// <summary>
        /// Creates an error with Forbidden code and given message
        /// </summary>
        public static AuthorizationError Forbidden(string message)
        {
            return new AuthorizationError(message) { Code = Constants.ForbiddenCode };
        }

        /// <summary>
        /// Throws generic "Access denied" error
        /// </summary>
        [Obsolete("Use `throw AuthorizationError.Forbidden()`", DiagnosticId = "VC0005", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        public static void ThrowAccessDeniedError()
        {
            throw Forbidden();
        }

        /// <summary>
        /// Throws "Password expired" error
        /// </summary>
        [Obsolete("Use `throw AuthorizationError.PasswordExpired()`", DiagnosticId = "VC0005", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        public static void ThrowPasswordExpiredError()
        {
            throw PasswordExpired();
        }

        /// <summary>
        /// Throws "User locked" error
        /// </summary>
        [Obsolete("Use `throw AuthorizationError.UserLocked()`", DiagnosticId = "VC0005", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        public static void ThrowUserLockedError()
        {
            throw UserLocked();
        }
    }
}
