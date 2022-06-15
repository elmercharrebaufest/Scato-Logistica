using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.AfipCTGWebService;
using Molinos.Scato.Servicios.AfipCPDigitalService;
using Molinos.Scato.Servicios.AfipWebService;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Conversiones.Impl;
using Molinos.Scato.Servicios.GestionarCartasDePortePE;
using Molinos.Scato.Servicios.Impl;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Servicios.ServicioImpresion;
using Molinos.Scato.Servicios.ServiciosSap;
using Molinos.Scato.Servicios.Urenport;
using Ninject.Modules;
using System.Data.Entity;
using System.Net.Http;
using System.ServiceModel;

namespace Molinos.Scato.Dependencias
{
    public class ServiciosWebNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<DbContext>().To<ScatoDbContext>().InScope(ctx => OperationContext.Current);
            Bind<IRepositorio>().To<RepositorioEF>().InScope(ctx => OperationContext.Current);

            Bind<IConversor>().To<ConversorAutoMapper>().InSingletonScope();

            Bind<IServicioRepositorio, ServicioRepositorio>().To<ServicioRepositorio>().InScope(ctx => OperationContext.Current);
            Bind<IServicioWorkflows, ServicioWorkflows>().To<ServicioWorkflows>().InScope(ctx => OperationContext.Current);
            Bind<IServicioComandos, ServicioComandos>().To<ServicioComandos>().InSingletonScope();
            Bind<IServicioSapAsincronico, ServicioSapAsincronico>().To<ServicioSapAsincronico>().InScope(ctx => OperationContext.Current);
            Bind<ICalculadoraDescuento, CalculadoraDescuento>().To<CalculadoraDescuento>().InScope(ctx => OperationContext.Current);
            Bind<IConfiguracionProvider, ConfiguracionProvider>().To<ConfiguracionProvider>().InSingletonScope();
            Bind<IAccesoWsCtg, AccesoWsCtg>().To<AccesoWsCtg>();
            Bind<IFirmaProvider, FirmaProvider>().To<FirmaProvider>().InSingletonScope();
            Bind<IServicioImpresorFactory, ServicioImpresorFactory>().To<ServicioImpresorFactory>().InSingletonScope();
            Bind<IServicioMercadoPago, ServicioMercadoPago>().To<ServicioMercadoPago>().InScope(ctx => OperationContext.Current);
            Bind<IServicioCircular, ServicioCircular>().To<ServicioCircular>().InScope(ctx => OperationContext.Current);
            Bind<HttpClient>().ToSelf().InSingletonScope();
            Bind<IAdministradorDeCalles, AdministradorDeCalles>().To<AdministradorDeCalles>().InScope(ctx => OperationContext.Current);
            
            this.BindChannelFactory<IServicioNotificarUsuario>("ServicioNotificarUsuario");
            Bind<IServicioEstadoPuesto, ServicioEstadoPuesto>().To<ServicioEstadoPuesto>().InScope(ctx => OperationContext.Current);
            Bind<ICache, Cache>().To<Cache>().InSingletonScope();

            this.BindChannelFactory<LoginCMS>("LoginCms");
            this.BindChannelFactory<CTGServicePortType>("CTGServiceHttpSoap11Endpoint");
            this.BindChannelFactory<ZSDWS_SCATO>("ZSDWS_SCATO", "SapServiceUsername", "SapServicePassword");
            this.BindChannelFactory<WaybillManagementPODv2>("WaybillManagementPODImplPort", "MonsantoServiceUsername", "MonsantoServicePassword");
            this.BindChannelFactory<calpesSoap>("calpesSoap");
            this.BindChannelFactory<IServicioOrquestador>("Orquestador");
            this.BindChannelFactory<IServicioImpresion>("ServicioImpresion");
            
            this.BindChannelFactory<CpePortType>("CpeEndPoint");


        }
    }
}
