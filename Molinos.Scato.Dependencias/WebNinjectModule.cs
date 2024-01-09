using System.Linq;
using System.Reflection;
using Molinos.Scato.Actividades.Interfaces;
using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.GestionarCartasDePortePE;
using Molinos.Scato.Servicios.Impl;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Servicios.ServiciosSap;
using Ninject.Modules;

namespace Molinos.Scato.Dependencias
{
    public class WebNinjectModule : NinjectModule
    {
        public override void Load()
        {
            /*
             * Para la comunicación con la capa de servicios se pueden usar dos formas:
             *  - Directa: referencia directa a las dlls de servicio
             *  - WCF:  se accede a la capa de servicios a traves de servicios WCF
             * Para cambiar de metodo ce comunicacion, basta con intercambiar las configuraciones de aca abajo.
            */
            // Comunicacion Directa 
            Bind<IConfiguracionProvider, ConfiguracionProvider>().To<ConfiguracionProvider>().InSingletonScope();
            // Fin comunicacion directa

            // Comunicacion por WCF
            this.BindChannelFactory<IServicioNotificarUsuario>("ServicioNotificarUsuario");
            this.BindChannelFactory<IServicioRepositorio>("ServicioRepositorio");
            this.BindChannelFactory<IServicioWorkflows>("ServicioWorkflows");
            this.BindChannelFactory<IServicioComandos>("ServicioComandos");
            this.BindChannelFactory<IServicioSuscriptor>("ServicioSuscriptor");
            this.BindChannelFactory<IListaDeWorkflows>("ListaDeWorkflows");
            this.BindChannelFactory<IServicioSapAsincronico>("ServicioSapAsincronico");
            this.BindChannelFactory<IFirmaProvider>("FirmaProvider");
            this.BindChannelFactory<IServicioEstadoPuesto>("ServicioEstadoPuesto");
            this.BindChannelFactory<IServicioLlamadoAutomatico>("ServicioLlamadoAutomatico");

            Bind(typeof(IServicioActividadFactory<>)).To(typeof(ServicioActividadFactory<>)).InSingletonScope();
            Bind(typeof(IServicioComandosFactory)).To(typeof(ServicioComandosFactory)).InSingletonScope();
            Bind(typeof(IServicioRepositorioFactory)).To(typeof(ServicioRepositorioFactory)).InSingletonScope();

            BindServiciosActividad();

            this.BindChannelFactory<ZSDWS_SCATO>("ZSDWS_SCATO", "SapServiceUsername", "SapServicePassword");
            this.BindChannelFactory<WaybillManagementPODv2>("WaybillManagementPODImplPort", "MonsantoServiceUsername", "MonsantoServicePassword");
            this.BindChannelFactory<IServicioOrquestador>("Orquestador");
        }

        private void BindServiciosActividad()
        {
            // Hace el binding de todas las interfaces de actividades
            var servicios = typeof (ICaladoService).Assembly.GetTypes()
                                                   .Where(t => t.Namespace == typeof (ICaladoService).Namespace);

            var metodo = typeof (ExtensionesNinject).GetMethod("BindWorkflowChannelFactory",
                                                               BindingFlags.NonPublic | BindingFlags.Static);

            foreach (var servicio in servicios)
            {
                metodo.MakeGenericMethod(servicio).Invoke(null, new object[] {this});
            }
        }
    }
}
