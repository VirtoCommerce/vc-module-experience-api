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
        public async Task Execute<TParameter>(TParameter parameter) where TParameter : class
        {
            var pipeline = _serviceProvider.GetService(typeof(GenericPipeline<TParameter>)) as GenericPipeline<TParameter>;
            if (pipeline == null)
            {
                throw new OperationCanceledException($"pipeline {typeof(GenericPipeline<TParameter>)} is not registered");
            }
            await pipeline.Execute(parameter);
        }
    }
}
