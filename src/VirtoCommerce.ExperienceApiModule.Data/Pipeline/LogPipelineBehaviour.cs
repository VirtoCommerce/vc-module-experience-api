using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace VirtoCommerce.ExperienceApiModule.Data.Pipeline
{
    public class LogPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger _logger;
        public LogPipelineBehaviour(ILogger<LogPipelineBehaviour<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            _logger.LogInformation($"-- Handling Request {request.ToString()}");
            var response = await next();
            _logger.LogInformation($"-- Finished Request {request.ToString()}");
            return response;
        }
    }

}
