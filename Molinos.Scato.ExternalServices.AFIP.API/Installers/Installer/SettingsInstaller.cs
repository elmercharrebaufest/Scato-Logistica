using Microsoft.Extensions.Options;
using Molinos.Scato.ExternalServices.DTO;

namespace Molinos.Scato.ExternalServices.AFIP.API.Installers
{
    public class SettingsInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var appSettingsSection = configuration.GetSection("AppSettings");
            services.Configure<AppSetting>(appSettingsSection);
            services.AddSingleton(cfg => cfg.GetService<IOptions<AppSetting>>().Value);
        }
    }
}