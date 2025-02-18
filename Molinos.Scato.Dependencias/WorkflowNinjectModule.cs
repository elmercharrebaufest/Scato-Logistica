using Molinos.Scato.Actividades.Servicios;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Behavior;
using Molinos.Scato.Servicios.ComplianceWebServiceV2;
using Molinos.Scato.Servicios.GestionarCartasDePortePE;
using Molinos.Scato.Servicios.ServiciosSap;
using Ninject.Modules;

namespace Molinos.Scato.Dependencias
{
    public class WorkflowNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind(typeof(IServicioActividadFactory<>)).To(typeof(ServicioActividadFactory<>)).InSingletonScope();

            this.BindChannelFactory<IServicioWorkflows>("ServicioWorkflows");
            this.BindChannelFactory<IFirmaProvider>("FirmaProvider");
            this.BindChannelFactory<IServicioRepositorio>("ServicioRepositorio");
            this.BindChannelFactory<IServicioComandos>("ServicioComandos");
            this.BindChannelFactory<IServicioNotificarUsuario>("ServicioNotificarUsuario");
            this.BindChannelFactory<IServicioSapAsincronico>("ServicioSapAsincronico");

            this.BindChannelFactory<ZSDWS_SCATO>("ZSDWS_SCATO", "SapServiceUsername", "SapServicePassword");

            this.BindChannelFactory<WaybillManagementPODv2>("WaybillManagementPODImplPort", "MonsantoServiceUsername", "MonsantoServicePassword", new BehaviorMonsanto());

            this.BindChannelFactory<DatosPort>("DatosPortV2");
            this.BindChannelFactory<Servicios.ComplianceWebService.DatosPort>("DatosPort");
        }
    }
}
