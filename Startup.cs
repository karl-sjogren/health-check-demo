using System;
using System.ServiceProcess;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace health_check_demo {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            services.AddHealthChecks()

            #region Enkel healthcheck
                .AddCheck("Alltid soligt!", () => HealthCheckResult.Healthy("Det är soligt!"), tags: new[] { "basic" })
            #endregion

            #region Failande healthcheck
                .AddCheck("Aldrig soligt...", () => HealthCheckResult.Degraded("Det är molnigt.."), tags: new[] { "fail" })
            #endregion

            #region SQL healthcheck
                .AddSqlServer(Configuration["ConnectionStrings:DefaultConnection"], tags: new[] { "sql" })
            #endregion

            #region Special-healthcheck
                .AddCheck<StackOverflowHealthCheck>("Stack Overflow", tags: new[] { "custom" })
            #endregion

            #region En bunt med healthchecks
                .AddDiskStorageHealthCheck(x => x.AddDrive("C:\\"), tags: new[] { "misc" })
                .AddElasticsearch("http://10.137.40.5:9200/", tags: new[] { "misc" })
                .AddPingHealthCheck(x => x.AddHost("www.vk.se", 1000), tags: new[] { "misc" })
                .AddWindowsServiceHealthCheck("VamClientServiceVer2", x => x.Status == ServiceControllerStatus.Running, tags: new[] { "misc" })
            #endregion

            #region Semikolon
                ;
            #endregion

            #region Publisher
            services.Configure<HealthCheckPublisherOptions>(options => {
                options.Delay = TimeSpan.FromSeconds(10);
            });

            services.AddSingleton<IHealthCheckPublisher, HealthCheckPublisher>();
            #endregion

            #region UI
            services.AddHealthChecksUI();
            #endregion

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints => {
                #region Tom healthcheck
                endpoints.MapHealthChecks("/health", new HealthCheckOptions() {
                    Predicate = (check) => check.Tags.Contains("empty")
                });
                #endregion

                #region Enkel healthcheck
                endpoints.MapHealthChecks("/health_basic", new HealthCheckOptions() {
                    Predicate = (check) => check.Tags.Contains("basic")
                });
                #endregion

                #region Enkel healthcheck
                endpoints.MapHealthChecks("/health_fail", new HealthCheckOptions() {
                    Predicate = (check) => check.Tags.Contains("fail")
                });
                #endregion

                #region SQL healthcheck
                endpoints.MapHealthChecks("/health_sql", new HealthCheckOptions() {
                    Predicate = (check) => check.Tags.Contains("sql")
                });
                #endregion

                #region Alla healthchecks
                endpoints.MapHealthChecks("/health_all", new HealthCheckOptions {
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                #endregion

                #region UI
                endpoints.MapHealthChecksUI(x => x.UIPath = "/ui");
                #endregion

                #region Annat skräp
                endpoints.MapGet("/", async context => {
                    context.Response.ContentType = "text/html; charset=utf-8";
                    await context.Response.WriteAsync("<span style=\"font-size: 96px\">🦒🦓🐘</span>");
                });
                #endregion
            });
        }
    }
}
