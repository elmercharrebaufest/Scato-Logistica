using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Impl;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Modules;
using System.Linq;
using System.Reflection;

namespace Molinos.Scato.Dependencias
{
    public class WebMobileNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind(typeof(IServicioActividadFactory<>)).To(typeof(ServicioActividadFactory<>)).InSingletonScope();
            Bind<IConfiguracionProvider, ConfiguracionProvider>().To<ConfiguracionProvider>().InSingletonScope();
            this.BindChannelFactory<IServicioRepositorio>("ServicioRepositorio");
            this.BindChannelFactory<IServicioComandos>("ServicioComandos");
            this.BindChannelFactory<IListaDeWorkflows>("ListaDeWorkflows");
            this.BindChannelFactory<IFirmaProvider>("FirmaProvider");
            this.BindChannelFactory<IServicioOrquestador>("Orquestador");
            BindServiciosActividad();

        }

        private void BindServiciosActividad()
        {
            // Hace el binding de todas las interfaces de actividades
            var servicios = typeof(ICaladoService).Assembly.GetTypes()
                                                   .Where(t => t.Namespace == typeof(ICaladoService).Namespace);

            var metodo = typeof(ExtensionesNinject).GetMethod("BindWorkflowChannelFactory",
                                                               BindingFlags.NonPublic | BindingFlags.Static);

            foreach (var servicio in servicios)
            {
                metodo.MakeGenericMethod(servicio).Invoke(null, new object[] { this });
            }
        }
    }
}
