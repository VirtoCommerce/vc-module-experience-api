using System;
using System.Collections;
using GraphQL;
using VirtoCommerce.ExperienceApiModule.Core.Helpers;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure
{
    public class LockError : ExecutionError
    {
        public LockError(string message) : base(message) => Code = Constants.LockedCode;
        public LockError(string message, IDictionary data) : base(message, data) => Code = Constants.LockedCode;
        public LockError(string message, Exception exception) : base(message, exception) => Code = Constants.LockedCode;
    }
}
