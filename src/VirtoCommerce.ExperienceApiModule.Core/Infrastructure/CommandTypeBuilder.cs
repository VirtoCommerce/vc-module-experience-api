using System;
using Microsoft.Extensions.DependencyInjection;

namespace VirtoCommerce.ExperienceApiModule.Core.Infrastructure
{
    public class CommandTypeBuilder : ICommandTypeBuilder
    {
        public IServiceCollection Services { get; }

        public Type CommandType { get; }

        public CommandTypeBuilder(IServiceCollection services, Type commandType)
        {
            Services = services;
            CommandType = commandType;
        }
    }
}
