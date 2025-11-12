using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Molinos.Scato.Dominio.Dto.HealthCheck
{
    public class HealthCheckConfigDto
    {
        public string Method { get; set; } = "GET";
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, string> QueryParameters { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        public string Body { get; set; }
        public string ContentType { get; set; }
        public int TimeoutMs { get; set; } = 5000;
        public HashSet<int> ExpectedStatusCodes { get; set; } = new HashSet<int> { 200 };
        public string SuccessContains { get; set; }
        public string SuccessJsonPath { get; set; }

        public static HealthCheckConfigDto FromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new HealthCheckConfigDto();

            try
            {
                var cfg = JsonConvert.DeserializeObject<HealthCheckConfigDto>(json);

                cfg.Headers = cfg.Headers ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                cfg.QueryParameters = cfg.QueryParameters ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                if (cfg.ExpectedStatusCodes == null || !cfg.ExpectedStatusCodes.Any())
                    cfg.ExpectedStatusCodes = new HashSet<int> { 200 };
                else
                {
                    cfg.ExpectedStatusCodes = new HashSet<int>(cfg.ExpectedStatusCodes.Where(c => c >= 100 && c <= 599));
                    if (!cfg.ExpectedStatusCodes.Any())
                        cfg.ExpectedStatusCodes = new HashSet<int> { 200 };
                }

                if (cfg.TimeoutMs <= 0)
                    cfg.TimeoutMs = 5000;

                if (string.IsNullOrWhiteSpace(cfg.Method))
                    cfg.Method = "GET";

                return cfg;
            }
            catch
            {
                return new HealthCheckConfigDto();
            }
        }
    }
}