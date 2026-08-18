using Hangfire;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Validations;
using Molinos.Scato.Dominio.Validations.Interfaces;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.AfipCPDigitalService;
using Molinos.Scato.Servicios.AfipCTGWebService;
using Molinos.Scato.Servicios.AfipWebService;
using Molinos.Scato.Servicios.Almacenamiento.Impl;
using Molinos.Scato.Servicios.Almacenamiento.Interfaces;
using Molinos.Scato.Servicios.ColasFIFO.Impl;
using Molinos.Scato.Servicios.ColasFIFO.Interfaces;
using Molinos.Scato.Servicios.ComplianceWebServiceV2;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Conversiones.Impl;
using Molinos.Scato.Servicios.DataAgroService;
using Molinos.Scato.Servicios.Dummy;
using Molinos.Scato.Servicios.Estrategias;
using Molinos.Scato.Servicios.GestionarCartasDePortePE;
using Molinos.Scato.Servicios.Imp;
using Molinos.Scato.Servicios.Impl;
using Molinos.Scato.Servicios.Impl.Hangfire;
using Molinos.Scato.Servicios.Interfaces;
using Molinos.Scato.Servicios.Orquestador;
using Molinos.Scato.Servicios.Procesamiento;
using Molinos.Scato.Servicios.ServicioImpresion;
using Molinos.Scato.Servicios.ServiciosSap;
using Molinos.Scato.Servicios.Urenport;
using Ninject;
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
            Bind<IFirmaProvider, FirmaProvider>().To<FirmaProvider>().InScope(ctx => OperationContext.Current);
            Bind<IServicioImpresorFactory, ServicioImpresorFactory>().To<ServicioImpresorFactory>().InSingletonScope();
            Bind<IServicioMercadoPago, ServicioMercadoPago>().To<ServicioMercadoPago>().InScope(ctx => OperationContext.Current);
            Bind<IServicioCircular, ServicioCircular>().To<ServicioCircular>().InScope(ctx => OperationContext.Current);
            Bind<HttpClient>().ToSelf().InSingletonScope();
            Bind<IAdministradorDeCalles, AdministradorDeCalles>().To<AdministradorDeCalles>().InScope(ctx => OperationContext.Current);
            Bind<IServicioEstadoPuesto, ServicioEstadoPuesto>().To<ServicioEstadoPuesto>().InScope(ctx => OperationContext.Current);
            Bind<ICache, Cache>().To<Cache>().InSingletonScope();
            Bind<IServicioLlamadoAutomatico, ServicioLlamadoAutomatico>().To<ServicioLlamadoAutomatico>().InScope(ctx => OperationContext.Current);
            Bind<IServicioSincronizacionVisec, ServicioSincronizacionVisec>().To<ServicioSincronizacionVisec>().InScope(ctx => OperationContext.Current);
            Bind<IServicioSincronizacionPay, ServicioSincronizacionPay>().To<ServicioSincronizacionPay>().InScope(ctx => OperationContext.Current);
            Bind<IServicioOperaciones, ServicioOperaciones>().To<ServicioOperaciones>();
            Bind<IValidatorEntity<OrdenCargaInternaFasonDto>>().To<OrdenCargaInternaFasonValidator>();
            Bind<IExternalServiceException, ExternalServiceException>().To<ExternalServiceException>();
            Bind<IRestClientFactory, RestClientFactory>().To<RestClientFactory>().InSingletonScope();
            Bind<IValidatorEntity<OrdenCargaFasDto>>().To<OrdenCargaFasValidator>();
            Bind<ICategorizadorVehiculo, CategorizadorVehiculo>().To<CategorizadorVehiculo>().InScope(ctx => OperationContext.Current);
            Bind<IServicioSincronizacionBandaHorariaStopRechazados, ServicioSincronizacionBandaHorariaStopRechazados>().To<ServicioSincronizacionBandaHorariaStopRechazados>().InScope(ctx => OperationContext.Current);

            Bind<IServicioHealthCheck, ServicioHealthCheck>().To<ServicioHealthCheck>().InScope(ctx => OperationContext.Current);
            Bind<IBackgroundJobClient>().To<BackgroundJobClient>().InSingletonScope();
            Bind<IServicioHangfireQueue, ServicioHangfireQueue>().To<ServicioHangfireQueue>().InScope(ctx => OperationContext.Current);
            Bind<IHangfireQueue, HangfireQueue>().To<HangfireQueue>().InScope(ctx => OperationContext.Current);

            Bind<IMarcaDeTiempo, MarcaDeTiempo>().To<MarcaDeTiempo>().InScope(ctx => OperationContext.Current);
            Bind<IColaIdentificacionVehicular>().To<SqlColaIdentificacionVehicular>().InScope(ctx => OperationContext.Current);
            Bind<IAlmacenamientoFotos>().To<FileSystemAlmacenamientoFotos>().InScope(ctx => OperationContext.Current);

            this.BindChannelFactory<IServicioNotificarUsuario>("ServicioNotificarUsuario");
            this.BindChannelFactory<LoginCMS>("LoginCms");
            this.BindChannelFactory<CTGServicePortType>("CTGServiceHttpSoap11Endpoint");
            this.BindChannelFactory<ZSDWS_SCATO>("ZSDWS_SCATO", "SapServiceUsername", "SapServicePassword");
            this.BindChannelFactory<WaybillManagementPODv2>("WaybillManagementPODImplPort", "MonsantoServiceUsername", "MonsantoServicePassword");
            this.BindChannelFactory<calpesSoap>("calpesSoap");
            this.BindChannelFactory<IServicioOrquestador>("Orquestador");
            this.BindChannelFactory<IServicioImpresion>("ServicioImpresion");
            this.BindChannelFactory<CpePortType>("CpeEndPoint");
            this.BindChannelFactory<DatosPort>("DatosPortV2");

            Bind<IBalanzadaContext>().To<BalanzadaContext>().InTransientScope();
            Bind<IBalanzadaStrategy>().To<BalanzadaStrategy>().InTransientScope();
            Bind<IBalanzadaStrategy>().To<BalanzadaInicioStrategy>().InTransientScope();
            Bind<IBalanzadaStrategy>().To<BalanzadaErrorStrategy>().InTransientScope();
            Bind<IBalanzadaStrategy>().To<BalanzadaFinStrategy>().InTransientScope();
            Bind<IServicioCarga, ServicioCarga>().To<ServicioCarga>().InScope(ctx => OperationContext.Current);

            Bind<IReglaTasaMunicipal>().To<ReglaTasaMunicipalGranos>();
            Bind<IReglaTasaMunicipal>().To<ReglaTasaMunicipalNoGranos>();
            Bind<IReglaTasaMunicipal>().To<ReglaTasaMunicipalAmbos>();
            Bind<IProcesadorComando>().To<ProcesadorVerificarPagoTasaMunicipal>().InSingletonScope();
            var servicioRepositorio = Kernel.Get<IServicioRepositorio>();
            ConfigurarServicioDataAgro(servicioRepositorio);
        }

        private void ConfigurarServicioDataAgro(IServicioRepositorio servicioRepositorio)
        {
            var configuracion = servicioRepositorio.ObtenerConfiguracionGeneral(
                Constantes.ConfiguracionGeneral.Pantalla.ConsultaDataAgroVisec,
                Constantes.ConfiguracionGeneral.ConsultaDataAgroVisec.DummyActivo);

            bool usarDummy = !string.IsNullOrEmpty(configuracion?.Valor)
                              && bool.TryParse(configuracion.Valor, out bool dummyActivo)
                              && dummyActivo;

            if (usarDummy)
            {
                Bind<IDataAgroServices>().To<DummyDataAgroServices>();
            }
            else 
            {
                this.BindChannelFactory<IDataAgroServices>("DataAgroServices", "DataAgroServiceUsername", "DataAgroServicePassword");
            }
        }
        
    }
}