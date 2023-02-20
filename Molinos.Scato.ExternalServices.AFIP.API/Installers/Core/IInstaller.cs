namespace Molinos.Scato.ExternalServices.AFIP.API.Installers
{
    public interface IInstaller
    {
        void InstallServices(IServiceCollection services, IConfiguration configuration);
    }
}