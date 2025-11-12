using Molinos.Scato.Dominio.Dto.HealthCheck;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios.Interfaces;
using Ninject.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Molinos.Scato.Servicios.Imp
{
    public class ServicioHealthCheck : IServicioHealthCheck
    {
        private readonly HttpClient httpClient;
        private readonly ILogger logger;

        public ServicioHealthCheck(HttpClient httpClient, ILogger logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        public async Task<HealthCheckResult> CheckAsync(MonitoreoServicioExternoDto service, CancellationToken cancellationToken)
        {
            var result = new HealthCheckResult();

            var cfg = HealthCheckConfigDto.FromJson(service.HealthCheckConfig);
            var timeoutMs = cfg.TimeoutMs;
            var method = cfg.Method;

            using (var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                cts.CancelAfter(TimeSpan.FromMilliseconds(timeoutMs));

                try
                {
                    var builder = new UriBuilder(service.HealthCheckUrl);
                    var query = System.Web.HttpUtility.ParseQueryString(builder.Query);

                    foreach (var qp in cfg.QueryParameters)
                        query[qp.Key] = qp.Value;

                    builder.Query = query.ToString();

                    using (var request = new HttpRequestMessage(new HttpMethod(method), builder.Uri))
                    {
                        foreach (var h in cfg.Headers)
                            request.Headers.TryAddWithoutValidation(h.Key, h.Value);

                        if (!string.IsNullOrEmpty(cfg.Body))
                        {
                            var contentType = string.IsNullOrWhiteSpace(cfg.ContentType) ? "application/json" : cfg.ContentType;
                            request.Content = new StringContent(cfg.Body, System.Text.Encoding.UTF8, contentType);
                        }
                        logger.Debug($"Request HealthCheck {request} \n Body: {cfg.Body}");
                        
                        using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cts.Token).ConfigureAwait(false))
                        {
                            logger.Debug($"Response HealthCheck {response}");
                            if (cfg.ExpectedStatusCodes.Contains((int)response.StatusCode))
                                result.Status = HealthCheckStatus.Conectado;
                            else
                                result.Status = HealthCheckStatus.Desconectado;

                            result.Message = $"Consulta realizada con codigo estado {response.StatusCode}";
                        }
                    }
                }
                catch (TaskCanceledException)
                {
                    result.Status = HealthCheckStatus.Desconectado;
                    result.Message = "Timeout";
                }
                catch (HttpRequestException ex)
                {
                    result.Status = HealthCheckStatus.Desconectado;
                    result.Message = ex.Message;
                }
                catch (Exception ex)
                {
                    result.Status = HealthCheckStatus.Desconocido;
                    result.Message = ex.Message;
                }
            }

            return result;
        }
    }
}