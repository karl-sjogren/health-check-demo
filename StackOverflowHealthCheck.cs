using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace health_check_demo {
    public class StackOverflowHealthCheck : IHealthCheck {
        private const string CheckUrl = "https://stackoverflow.com/";
        private readonly HttpClient _httpClient;
        private readonly ILogger<StackOverflowHealthCheck> _logger;

        public StackOverflowHealthCheck(IHttpClientFactory httpClientFactory, ILogger<StackOverflowHealthCheck> logger) {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) {
            try {
                var result = await _httpClient.GetAsync(CheckUrl);
                result.EnsureSuccessStatusCode();
            } catch(Exception e) {
                return new HealthCheckResult(HealthStatus.Unhealthy, "Failed to fetch start page for Stack Overflow, development is halted.", e);
            }

            return new HealthCheckResult(HealthStatus.Healthy, "Successfully got start page for Stack Overflow, development can continue.");
        }
    }
}