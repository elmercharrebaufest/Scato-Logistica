namespace  Molinos.Scato.API.WebAPI.Installers.Core
{
    public interface IInstaller
    {
        void InstallServices(IServiceCollection services, IConfiguration configuration);
    }
}