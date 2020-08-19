using Microsoft.Extensions.Options;
using PipelineNet.MiddlewareResolver;
using PipelineNet.Pipelines;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.Core.Pipelines
{
    public class GenericPipeline<TParameter> : AsyncPipeline<TParameter> where TParameter : class
    {
        public GenericPipeline(IOptions<GenericPipelineOptions<TParameter>> Options, IMiddlewareResolver MiddlewareResolver) : base(MiddlewareResolver)
        {
            Options.Value.Middlewares.Apply(Middleware => Add(Middleware));
        }

    }
}
