using System;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure.Authorization
{
    public sealed class AuthorizationException : Exception
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
