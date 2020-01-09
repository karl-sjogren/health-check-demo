using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace health_check_demo {
    public class HealthCheckPublisher : IHealthCheckPublisher {
        private readonly ILogger<HealthCheckPublisher> _logger;

        public HealthCheckPublisher(ILogger<HealthCheckPublisher> logger) {
            _logger = logger;
        }
        public Task PublishAsync(HealthReport report, CancellationToken cancellationToken) {
            if(report.Status == HealthStatus.Healthy)
                _logger.LogInformation("Publishing health check report with status " + report.Status);
            else if(report.Status == HealthStatus.Degraded)
                _logger.LogWarning("Publishing health check report with status " + report.Status);
            else if(report.Status == HealthStatus.Unhealthy)
                _logger.LogError("Publishing health check report with status " + report.Status);

            return Task.CompletedTask;
        }
    }
}