using Microsoft.OpenApi.Models;
using  Molinos.Scato.API.WebAPI.Installers.Core;

namespace  Molinos.Scato.API.WebAPI.Installers.Installer
{
    public class SwagerInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Scato Service API",
                    Version = "v1"
                });
            });
        }
    }
}