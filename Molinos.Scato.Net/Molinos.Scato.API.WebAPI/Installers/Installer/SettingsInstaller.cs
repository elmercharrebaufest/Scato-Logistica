using Microsoft.Extensions.Options;
using Molinos.Scato.API.CrossCutting.DTO;
using  Molinos.Scato.API.WebAPI.Installers.Core;

namespace  Molinos.Scato.API.WebAPI.Installers.Installer
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