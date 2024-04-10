using System;

namespace VirtoCommerce.ExperienceApiModule.Core.Subscriptions
{
    public class GraphQLWebSocketOptions
    {
        public TimeSpan KeepAliveInterval { get; set; } = TimeSpan.FromSeconds(45);
    }
}
