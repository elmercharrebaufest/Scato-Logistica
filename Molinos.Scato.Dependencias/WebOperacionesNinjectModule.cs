using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Impl;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Modules;

namespace Molinos.Scato.Dependencias
{
    public class WebOperacionesNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IConfiguracionProvider, ConfiguracionProvider>().To<ConfiguracionProvider>().InSingletonScope();
            this.BindChannelFactory<IServicioRepositorio>("ServicioRepositorio");
            this.BindChannelFactory<IServicioComandos>("ServicioComandos");
            this.BindChannelFactory<IFirmaProvider>("FirmaProvider");
            this.BindChannelFactory<IServicioOrquestador>("Orquestador");
        }
    }
}
