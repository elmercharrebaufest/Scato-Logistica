using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.ExternalServices.DTO
{
    public class AppSetting
    {
        public ConnectionString? ConnectionStrings { get; set; }
        public JWTConfiguration? JWTConfigurations { get; set; }
    }

    public class ConnectionString
    {
        public string? DefaultConnection { get; set; }
    }

    public class JWTConfiguration
    {
        public string? Secret { get; set; }
        public int ExpirationTimeHours { get; set; }
    }
}
