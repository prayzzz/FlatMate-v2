using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace FlatMate.Module.Offers
{
    public class RestEaseRequestLogger
    {
        private readonly ILogger<RestEaseRequestLogger> _logger;

        public RestEaseRequestLogger(ILogger<RestEaseRequestLogger> logger)
        {
            _logger = logger;
        }

        public Task RequestModifier(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation(request.Method + ": " + request.RequestUri);
            }

            return Task.CompletedTask;
        }
    }
}