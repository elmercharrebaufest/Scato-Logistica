using Microsoft.OpenApi.Models;

namespace Molinos.Scato.ExternalServices.AFIP.API.Installers
{
    public class SwagerInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Scato Exteral Services API",
                    Version = "v1"
                });
            });
        }
    }
}