using System;
using System.Threading.Tasks;

namespace VirtoCommerce.ExperienceApiModule.Core.Pipelines
{
    public class GenericPipelineLauncher : IGenericPipelineLauncher
    {
        private readonly IServiceProvider _serviceProvider;
        public GenericPipelineLauncher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public Task Execute<TParameter>(TParameter parameter) where TParameter : class
        {
            if (_serviceProvider.GetService(typeof(GenericPipeline<TParameter>)) is not GenericPipeline<TParameter> pipeline)
            {
                throw new OperationCanceledException($"pipeline {typeof(GenericPipeline<TParameter>)} is not registered");
            }
            return pipeline.Execute(parameter);
        }
    }
}
