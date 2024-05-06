using System;
using Microsoft.Extensions.DependencyInjection;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure
{
    public interface ICommandTypeBuilder
    {
        IServiceCollection Services { get; }

        Type CommandType { get; }
    }
}
