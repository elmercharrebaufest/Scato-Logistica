using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Filtros;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Seguridad;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;

namespace Molinos.Scato.Servicios
{
    [ServiceContract(Namespace = "http://scato.molinos.com.ar")]
    public interface IServicioRepositorio
    {
        [OperationContract]
        TipoDocumentoIdentidadDto ObtenerTipoDocumentoIdentidad(int id);

        [OperationContract]
        ConversionCentroDto ObtenerConversionCentro(int camaraId, int centroId);

        [OperationContract]
        VariedadPorVinedoDto ObtenerVariedadPorVinedo(int id);

        [OperationContract]
        string ObtenerCodigoEstablecimientoPorGuid(Guid instanceId);

        [OperationContract]
        CaracteristicaDeCalidadDto ObtenerCaracteristicaDeCalidadPorMaterial(int materialId);

        [OperationContract]
        ListaPaginada<TipoDocumentoIdentidadDto> ListarPaginadoTiposDocumentoIdentidad(Paginacion paginacion);

        [OperationContract]
        PesoVagonesDto ObtenerPesoNetoTren(int cartaporteId);

        [OperationContract]
        ListaPaginada<VariedadPorVinedoDto> ListarPaginadoVariedadPorVinedo(int vinedoId, Paginacion paginacion);

        [OperationContract]
        IList<TipoDocumentoIdentidadDto> ListarTiposDocumentoIdentidad();

        [OperationContract]
        IList<TipoVehiculoBodegaDto> ListarTiposVehiculoBodega();

        [OperationContract]
        MaterialDto ObtenerMaterial(int id);

        [OperationContract]
        MaterialDto ObtenerMaterialPorCodigoSap(string codigo);

        [OperationContract]
        int ObtenerMaterialIdPorCodigoSap(string codigo);

        [OperationContract]
        AlmacenDto ObtenerAlmacen(int id);

        [OperationContract]
        string ObtenerAlmacenDescripcion(int id);

        [OperationContract]
        TaraRomaneoDto ObtenerTaraRomaneo(int id);

        [OperationContract]
        TalonarioDto ObtenerTalonario(int id);

        [OperationContract]
        CentroDto ObtenerCentro(int id);

        [OperationContract]
        bool ValidarLimiteDeCreditoVentaEnSAP(int centroId);

        [OperationContract]
        int? ObtenerTiempoMaximoCentro(int id);

        [OperationContract]
        string ObteneCodigoSapPorCentroId(int id);

        [OperationContract]
        CaracteristicasAnalizadasDto ObtenerCaracteristicasAnalizadasPorRecorridoId(int recorridoId);

        [OperationContract]
        CaracteristicasAnalizadasDto ObtenerCaracteristicasAnalizadasPorInstanceId(Guid instanceId);

        [OperationContract]
        int ObtenerCentroIdPorInstanceId(Guid instanceId);

        [OperationContract]
        bool TieneDescuentoPorHumedad(Guid instanceId);

        [OperationContract]
        bool TieneAnalisisDeCalidad(Guid instanceId);

        [OperationContract]
        BalanzaDto ObtenerBalanza(int id);

        [OperationContract]
        string ObtenerBalanzaNombre(int id);

        [OperationContract]
        IList<BalanzaDto> ListarTodasLasBalanzasActivas(int centroId);

        [OperationContract]
        IList<BalanzaDto> ListarTodasLasBalanzas(int centroId);

        [OperationContract]
        IList<BalanzaDto> ListarBalanzasActivas(int centroId, TipoVehiculo tipoVehiculo);

        [OperationContract]
        IList<BalanzaDto> ListarBalanzasActivasPorNombrePc(int centroId, string nombrePc, TipoVehiculo tipoVehiculo);

        [OperationContract]
        IList<BalanzaAutomaticaDto> ListarPuestosAutomaticosporCentro(int centroId);

        [OperationContract]
        BalanzaDto ObtenerBalanzaPorPuestoDeTrabajoAutomatico(int puestoId);

        [OperationContract]
        IList<NotificacionDto> ListarErrorBalanzas(List<int> puestosId);

        [OperationContract]
        IList<CentroInfoDto> BuscarCentros(string criteria);

        [OperationContract]
        CentroInfoDto BuscarCentro(string criteria);

        [OperationContract]
        IList<CentroDto> BuscarCentrosBodega(string criteria);

        [OperationContract]
        CentroDto BuscarCentroBodega(string criteria);

        [OperationContract]
        PrecintoDto ObtenerPrecinto(int id);

        [OperationContract]
        IList<AlmacenDto> ObtenerAlmacenesPorCentro(int centroId);

        [OperationContract]
        IList<TipoEmbalajeDto> ListarEmbalaje();

        [OperationContract]
        MaterialPorCentroDto ObtenerMaterialPorCentro(int centroId, int materialId);

        [OperationContract]
        MaterialPorCentroDto ObtenerMaterialPorCentroPorInstanceId(Guid instanceId);

        [OperationContract]
        CamaraDto ObtenerCamaraPorMaterialPorCentro(Guid instanceId);

        [OperationContract]
        ListaPaginada<MaterialDto> ListarPaginadoMateriales(string filtro, int centroId, Paginacion paginacion);

        [OperationContract]
        IList<MaterialPorCentroDto> BuscarMaterialesPorCentro(int centroId, string filtro, int cantidad = 20, string tipoCalle = null);

        [OperationContract]
        IList<string> BuscarDescripcionMaterialesPorCentro(int centroId, string filtro);

        [OperationContract]
        string ObtenerBodyPlanoDeCarga(int planoDeCargaId, EmbarqueDto embarque);

        [OperationContract]
        IList<AlmacenDto> ListarAlmacenes();

        [OperationContract]
        ListaPaginada<TaraRomaneoDto> ListarTaraRomaneos(string filtro, Paginacion paginacion);

        [OperationContract]
        IList<TaraRomaneoDto> ListarTaraRomaneosPorCentro(int centroId);

        [OperationContract]
        IList<AlmacenDto> ListarAlmacenesPorCentroYesSustentable(int centroId, bool esSustentable);

        [OperationContract]
        IList<AlmacenDto> ListarAlmacenesPorCentro(int centroId);

        [OperationContract]
        IList<AlmacenDto> ListarAlmacenesPorMaterialYCentro(int centroId, int materialId, bool esSustentable);

        [OperationContract]
        IList<MaterialDto> ListarMaterialesPorCamara(int camaraId);

        [OperationContract]
        IList<ConversionGrupoDto> ListarGruposPorCamara(int camaraId);

        [OperationContract]
        ListaPaginada<AlmacenDto> ListarPaginadoAlmacenes(string filtro, Paginacion paginacion, int centroId);

        [OperationContract]
        ListaPaginada<TaraRomaneoDto> ListarPaginadoTaraRomaneo(string filtro, Paginacion paginacion, int centroId);

        [OperationContract]
        ListaPaginada<TalonarioDto> ListarPaginadoTalonario(string filtro, Paginacion paginacion, int centroId);

        [OperationContract]
        IList<CentroDto> ListarCentros();

        [OperationContract]
        IList<CentroDto> ListarCentrosPorUsuario(string nombreUsuario);

        [OperationContract]
        ListaPaginada<CentroDto> ListarPaginadoCentros(Paginacion paginacion);

        [OperationContract]
        ListaPaginada<CentroDto> ListarPaginadoCentrosPorUsuario(string nombreUsuario, Paginacion paginacion);

        [OperationContract]
        IList<CamaraDto> ListarCamaras();

        [OperationContract]
        ListaPaginada<CamaraDto> ListarPaginadoCamaras(string filtro, Paginacion paginacion);

        [OperationContract]
        ListaPaginada<CategoriaDto> ListarPaginadoCategoria(string filtro, Paginacion paginacion);

        [OperationContract]
        CasilleroDto ObtenerCasillero(int id);

        [OperationContract]
        CasilleroDto ObtenerCasilleroPorNumeroYCentro(string numero, int centroId);

        [OperationContract]
        ListaPaginada<CasilleroDto> ListarPaginadoCasilleros(Paginacion paginacion, int centroId);

        [OperationContract]
        IList<CasilleroDto> ListarCasillerosPorCentro(int centroId);

        [OperationContract]
        IList<MicroMuestrasPorCasilleroDto> ListarMicroMuestrasPorCasilleroPorCentro(int centroId);

        [OperationContract]
        ListaPaginada<LiberacionDeCasillerosDto> ListarLiberacionDeCasilleros(LiberacionDeCasillerosDto filtro, Paginacion paginacion);

        [OperationContract]
        ListaPaginada<ConsultaCasilleroAntiguedadDto> ListarConsultaCasillerosPorAntiguedad(ConsultaCasilleroAntiguedadDto filtro, Paginacion paginacion);

        [OperationContract]
        ListaPaginada<ConsultaCasilleroDto> ListarConsultaCasillerosPorCasillero(ConsultaCasilleroDto filtro, Paginacion paginacion);

        [OperationContract]
        ListaPaginada<ConsultaCasilleroMuestraDto> ListarConsultaCasillerosPorMuestra(ConsultaCasilleroMuestraDto filtro, Paginacion paginacion);

        [OperationContract]
        CamaraDto ObtenerCamara(int id);

        [OperationContract]
        CategoriaDto ObtenerCategoria(int id);

        [OperationContract]
        IList<ProvinciaDto> ListarProvincias();

        [OperationContract]
        IList<LocalidadDto> ListarLocalidades();

        [OperationContract]
        IList<CategoriaDto> ListarCategorias();

        [OperationContract]
        IList<LocalidadDto> ListarLocalidadesPorProvincia(int provinciaId);

        [OperationContract]
        LocalidadDto ObtenerLocalidad(int id);

        [OperationContract]
        ListaPaginada<ChoferDto> ListarChoferes(string filtro, Paginacion paginacion);

        [OperationContract]
        ListaPaginada<CategoriaVehiculoDto> ListarCategoriaCamiones(string filtro, Paginacion paginacion);

        [OperationContract]
        CategoriaVehiculoDto BuscarCategoriaVehiculo(string patente, string acoplado, string acoplado2);

        [OperationContract]
        ListaPaginada<DocumentoExternoDto> ListarDocumentos(string filtro, Paginacion paginacion);

        [OperationContract]
        ChoferDto ObtenerChofer(int id);

        [OperationContract]
        CategoriaVehiculoDto ObtenerCategoriaVehiculo(int id);

        [OperationContract]
        KmPorProveedorDto ObtenerKmPorProveedor(int id);

        [OperationContract]
        ChoferDto ObtenerChoferPorCuit(string cuit);

        [OperationContract]
        Guid ObtenerInstanceIdPorPatente(string patente);

        [OperationContract]
        IList<ChoferDto> BuscarChoferes(ChoferFiltro filtro);

        [OperationContract]
        ChoferDto BuscarChofer(ChoferFiltro filtro);

        [OperationContract]
        IList<ChoferDto> BuscarChoferesGeneral(string filtro);

        [OperationContract]
        IList<TransportistaDto> BuscarTransportistas(string criteria);

        [OperationContract]
        TransportistaInfoDto BuscarTransportista(string criteria);

        [OperationContract]
        IList<TransportistaInfoDto> BuscarTransportistasPorCuit(string criteria);

        [OperationContract]
        ListaPaginada<TransportistaDto> ListarPaginadoTransportistas(string filtro, Paginacion paginacion);

        [OperationContract]
        IList<TransportistaDto> ListarTransportistas();

        [OperationContract]
        TransportistaDto ObtenerTransportista(int id);

        [OperationContract]
        TransportistaDto ObtenerTransportistaPorCuit(string cuit);

        [OperationContract]
        TransportistaDto ObtenerTransportistaPorRazonSocial(string razonSocial);

        [OperationContract]
        ClienteDto ObtenerCliente(int id);

        [OperationContract]
        ClienteDto ObtenerClientePorCodigoSap(string codigo);

        [OperationContract]
        ProveedorDto ObtenerProveedorPorCodigoSap(string codigoSap);

        [OperationContract]
        ListaPaginada<ExcepcionAlControlDto> ListarExcepcionesAlControl(string filtro, Paginacion paginacion);

        [OperationContract]
        ExcepcionAlControlDto ObtenerExcepcionAlControl(int id);

        [OperationContract]
        ListaPaginada<ExcepcionAlDescuentoDto> ListarExcepcionesAlDescuento(string filtro, Paginacion paginacion, int centroId);

        [OperationContract]
        ListaPaginada<KmPorProveedorDto> ListarKmPorProveedor(string filtro, Paginacion paginacion);

        [OperationContract]
        ExcepcionAlDescuentoDto ObtenerExcepcionAlDescuento(int id);

        [OperationContract]
        bool BuscarExcepcionAlControl(int materialId, int transportistaId, int centroId, DateTime fecha, int? centroDestinoId, int? clienteDestinoId);

        [OperationContract]
        ListaPaginada<InhabilitacionChoferDto> ListarInhabilitacionChoferes(string filtro, int centroId, Paginacion paginacion);

        [OperationContract]
        InhabilitacionChoferDto ObtenerInhabilitacionChofer(int id);

        [OperationContract]
        bool ChoferInhabilitado(int choferId, int centroId);

        [OperationContract]
        IEnumerable<InhabilitacionChoferDto> ListarInhabilitacionChofer(int choferId, int centroId);

        [OperationContract]
        ListaPaginada<InhabilitacionChoferDto> ListarInhabilitacionChoferPaginada(int choferId, int centroId, Paginacion paginacion);

        [OperationContract]
        ListaPaginada<TipoComercialDto> ListarPaginadoTiposComerciales(string filtro, Paginacion paginacion);

        [OperationContract]
        IList<TipoComercialDto> ListarTiposComerciales();

        [OperationContract]
        IList<TipoComercialDto> ListarTiposComercialesPorCentro(int centroId);

        [OperationContract]
        IList<TipoComercialDto> ListarTiposComercialesPorWfCodigo(string workflowCodigo);

        [OperationContract]
        IList<ExportacionDeArchivosSelectObjDto> ListarMaterialesConFiltro(int cantResultados, int pagina);

        [OperationContract]
        IList<TipoComprobanteOnccaDto> ListarTiposComprobantesOncca();

        [OperationContract]
        RecorridoDto ObtenerRecorridoPorGuid(Guid guid);

        [OperationContract]
        List<string> ObtenerUsuarioRechazoPorWorkflowInstance(Guid workflowInstance);

        [OperationContract]
        TipoDeWorkflow ObtenerTipoDeWorkflowPorGuid(Guid guid);

        [OperationContract]
        DatosDeInstanciaDto ObtenerDatosDeInstanciaPorGuid(Guid guid);

        [OperationContract]
        DatosDeInstanciaAltaCTGDto ObtenerDatosDeInstanciaAltaCTGPorGuid(Guid guid);

        [OperationContract]
        string ObtenerPatentePorGuid(Guid guid);

        [OperationContract]
        string ObtenerNumeroCartaPortePorGuid(Guid guid);

        [OperationContract]
        ValoresSapDto ObtenerRecorridoValoresSapPorGuid(Guid guid);

        [OperationContract]
        int ObtenerRecorridoIdPorGuid(Guid guid);

        [OperationContract]
        int ObtenerPesoMaximo(TipoVehiculo tipoVehiculo, int centroId);

        [OperationContract]
        TipoComercialDto ObtenerTipoComercial(int id);

        [OperationContract]
        ListaPaginada<TipoComercialPorWfDto> ListarTiposComercialesPorWf(string filtro, int centroId, Paginacion paginacion);

        [OperationContract]
        ListaPaginada<InhabilitacionCamionDto> ListarInhabilitacionCamiones(string filtro, int centroId, Paginacion paginacion);

        [OperationContract]
        InhabilitacionCamionDto ObtenerInhabilitacionCamion(int id);

        [OperationContract]
        bool CamionInhabilitado(string patente, int centroId);

        [OperationContract]
        IEnumerable<InhabilitacionCamionDto> ListarInhabilitacionCamion(string patente, int centroId);

        [OperationContract]
        ListaPaginada<InhabilitacionCamionDto> ListarInhabilitacionCamionPaginada(string patente, string patenteAcoplado, int centroId, Paginacion paginacion);

        [OperationContract]
        IList<PrecintoDto> ListarPrecintos(Guid instanceId);

        [OperationContract]
        ObservacionDto ObtenerObservacion(Guid instanceId);

        [OperationContract]
        IList<WorkflowDto> ListarWorkflowsPorCentro(int centroId);

        [OperationContract]
        IList<WorkflowDto> ListarWorkflowsCodigoPorCentro(int centroId);

        [OperationContract]
        WorkflowDto ObtenerWorkflow(int id);

        [OperationContract]
        IList<NotificacionDto> ListarNotificacionesPorGrupo(string grupo, int cantidad);

        [OperationContract]
        WorkflowDto ObtenerWorkflowPorCodigo(string codigo);

        [OperationContract]
        int ObtenerUltimaWorkflowDefinicionPorCordigo(string codigo);

        [OperationContract]
        WorkflowDefinicionDto ObtenerWorkflowDefinicion(int id);

        [OperationContract]
        bool WorkflowActivoConDefinicionActiva(string codigoWorkflow);

        [OperationContract]
        TipoDocumentoIngreso ObtenerTipoDocumentoIngresoPorRecorrido(int id);

        [OperationContract]
        RecorridoDto ObtenerRecorrido(int id);

        [OperationContract]
        IList<RecorridoDto> ObtenerRecorridoPorNumeroDocumento(string numero);

        [OperationContract]
        IList<RecorridoDto> ObtenerRecorridoNoRechazadoPorNumeroDocumento(string numero);

        [OperationContract]
        RecorridoDto ObtenerRecorridoPorNumeroDocumentoSap(string numero);

        [OperationContract]
        RecorridoDto ObtenerRecorridoPorNumeroCiu(string numero);

        [OperationContract]
        Guid ObtenerRecorridoGuidPorNumeroCiu(string numero);

        [OperationContract]
        IList<RecorridoDto> ListarRecorridoPorNumeroDocumentoYWorkflow(string numero, int workflowDefinicion);

        [OperationContract]
        OrdenCargaInternaDto ObtenerOrdenCargaInterna(int id);

        [OperationContract]
        EmbarqueDto ObtenerEmbarque(int id);

        [OperationContract]
        OrdenCargaInternaFasonDto ObtenerOrdenCargaInternaFason(int id);

        [OperationContract]
        bool ObtenerCodigoEstablecimientoEsDeMolinos(string codigoDeEstablecimiento);

        [OperationContract]
        OrdenCargaInternaFasonDto ObtenerOrdenCargaInternaFasonPorInstanceId(Guid id);

        [OperationContract]
        OrdenCargaInternaFasonDto ObtenerOrdenCargaInternaFasonPorNumero(string numero);

        [OperationContract]
        OrdenCargaInternaDto ObtenerOrdenCargaInternaPorInstanceId(Guid id);

        [OperationContract]
        ClienteDto ObtenerClientePorInstanceId(Guid id, TipoDocumentoIngreso tipo);

        [OperationContract]
        ListaPaginada<BalanzaDto> ListarPaginadoBalanza(string filtro, int centroId, Paginacion paginacion);

        [OperationContract]
        int ObtenerCantidadCalados(Guid instanceId);

        [OperationContract]
        CaladoDto ObtenerCaladoAnterior(Guid instanceId);

        [OperationContract]
        CaladoDto ObtenerCaladoPorGuid(Guid guid);

        [OperationContract]
        ListaPaginada<CaracteristicaDeCalidadDto> ListarCaracteristicasDeCalidadPaginado(Paginacion paginacion, string term, int centroId);

        [OperationContract]
        IList<CaracteristicaDeCalidadDto> ListarCaracteristicasDeCalidadPorMaterial(int materialId, int centroId);

        [OperationContract]
        IList<CaracteristicaDeCalidadDto> ListarCaracteristicasDeCalidadPorMaterialWorkflow(int materialId, int centroId, int workflowId);

        [OperationContract]
        IList<CaracteristicaDeCalidadPorWorkflowDto> ListarCaracteristicasDeCalidadPorWorkflow(int materialId, int centroId, int workflowId);

        [OperationContract]
        IList<CaracteristicaDeCalidadDto> ListarCaracteristicasDeCalidadPorMaterialSinHumedad(int materialId, int centroId);

        [OperationContract]
        ListaPaginada<CaracteristicaDeCalidadDto> ListarCaracteristicasDeCalidadPorMaterialPaginado(int materialId, int centroId, Paginacion paginacion);

        [OperationContract]
        IList<CaracteristicaDeCalidadDto> ListarCaracteristicasDeCalidadObligatorias(int materialId, int centroId);

        [OperationContract]
        CaracteristicaDeCalidadDto ObtenerCaracteristicaDeCalidadHumedad(int materialId, int centroId);

        [OperationContract]
        CaracteristicaDeCalidadDto ObtenerCaracteristicaDeCalidad(int id);

        [OperationContract]
        MotivoDto ObtenerMotivo(int id);

        [OperationContract]
        ListaPaginada<MotivoDto> ListarPaginadoMotivos(Paginacion paginacion);

        [OperationContract]
        IList<MotivoDto> ListarMotivos();

        [OperationContract]
        IList<MotivoReasignacionDeTarjetaDto> ListarMotivosReasignacionDeTarjeta();

        [OperationContract]
        HumedimetroDto ObtenerHumedimetro(int id);

        [OperationContract]
        IList<HumedimetroDto> ListarHumedimetros();

        [OperationContract]
        IList<HumedimetroDto> ListarHumedimetrosPorCentro(int centroId);

        [OperationContract]
        ListaPaginada<HumedimetroDto> ListarPaginadoHumedimetros(string filtro, int centroId, Paginacion paginacion);

        [OperationContract]
        ListaPaginada<BocaDestinoDto> ListarPaginadoBocasDestino(string filtro, Paginacion paginacion);

        [OperationContract]
        IList<BocaDestinoDto> ListarBocasDestino();

        [OperationContract]
        IList<ProveedorDto> ListarProveedoresConBocaDestino(string filtro);

        [OperationContract]
        ProveedorDto ObtenerProveedorConBocaDestino(string filtro);

        [OperationContract]
        BocaDestinoDto ObtenerBocaDestino(int id);

        [OperationContract]
        IList<CaladoPorCaracteristicaDto> ListarCaladoPorCaracteristicas(int caladoId);

        [OperationContract]
        IList<CalleDto> ObtenerCallesDeCallesPorRecorridoSegunMaterial(int materialId, int calleId, TipoCalidad calidadCamion, bool esSojaEPA, bool esSojaIMPO);

        [OperationContract]
        ListaPaginada<CaladoPorCaracteristicaDto> ListarPaginadoCaladoPorCaracteristica(int caladoId, Paginacion paginacion);

        [OperationContract]
        RolDto ObtenerRol(int id);

        [OperationContract]
        IList<RolDto> ListarRoles();

        [OperationContract]
        ListaPaginada<RolDto> ListarPaginadoRoles(string filtro, Paginacion paginacion);

        [OperationContract]
        IList<RecorridoDto> ListarRecorridosPorDocumento(string tipoDoc, string numeroDoc);

        [OperationContract]
        DatosRecorridoDto RecorridoPorTarjetaDeAcceso(string tarjetaDeAcceso);

        [OperationContract]
        RecorridoDto RecorridoSinPesosPorNumeroDeDocumento(TipoDocumentoIngreso tipo, string numeroDoc);

        [OperationContract]
        RecorridoDto ObtenerRecorridoPorDocumentoPatenteYCentro(string tipoDoc, string numeroDoc, string patente, int centroId);

        [OperationContract]
        IList<RecorridoDto> ListarRecorridosPorDocumentoYPatente(string tipoDoc, string numeroDoc, string patente);

        [OperationContract]
        ListaPaginada<RecorridoDto> ListarPaginadoRecorridosPorDocumentoPatenteYCentro(TipoDocumentoIngreso tipoDoc, string numeroDoc, string patente, string numeroTarjeta, int centroId, Paginacion paginacion);

        [OperationContract]
        PermisoDto ObtenerPermiso(int id);

        [OperationContract]
        IList<PermisoDto> ListarPermisos();

        [OperationContract]
        ListaPaginada<PermisoDto> ListarPaginadoPermisos(string filtro, Paginacion paginacion);

        [OperationContract]
        IList<PermisoDto> ListarPermisosPorUsuario(string nombreUsuario);

        [OperationContract]
        ExcepcionEnvioCamaraDto ObtenerExcepcionEnvioCamara(int id);

        [OperationContract]
        List<ExcepcionEnvioCamaraDto> ListarExcepcionEnvioCamara(int material, int tipoComercial, int proveedor, int entregador);

        [OperationContract]
        DocumentoDeImpresionDto ObtenerImpresiones(int id);

        [OperationContract]
        ListaPaginada<DocumentoDeImpresionDto> ListarPaginadoDocumentoDeImpresion(Paginacion paginacion, string filtro);

        [OperationContract]
        ListaPaginada<LoteListaDto> ListarPaginadoLote(BuscarLoteDto filtro, Paginacion paginacion);

        [OperationContract]
        ListaPaginada<LoteBiotecnologiaListaDto> ListarPaginadoBiotecnologiaLote(BuscarLoteBiotecnologiaDto filtro, Paginacion paginacion);

        [OperationContract]
        ListaPaginada<ArchivoDeMovimientosDto> ListarPaginadoArchivoDeMovimiento(BuscarArchivoDeMovimientoDto filtro, Paginacion paginacion);

        [OperationContract]
        IList<LocalidadDto> BuscarProcedencias(string criteria);

        [OperationContract]
        LocalidadDto BuscarProcedencia(string criteria);

        [OperationContract]
        ProvinciaDto ObtenerProvincia(int id);

        [OperationContract]
        List<ProveedorInfoDto> BuscarProveedoresVinedosTerceros(string criteria);

        [OperationContract]
        IList<VinedoDto> BuscarVinedosPropios(string criteria);

        [OperationContract]
        ProveedorInfoDto BuscarProveedorVinedoTercero(string criteria);

        [OperationContract]
        VinedoDto BuscarVinedoPropio(string criteria);

        [OperationContract]
        IList<ProveedorInfoDto> BuscarProveedores(string criteria, TiposProveedor tipo);

        [OperationContract]
        ProveedorInfoDto BuscarProveedor(string criteria, TiposProveedor tipo);

        [OperationContract]
        IList<ProveedorInfoDto> BuscarProveedoresPorCuit(string cuit, TiposProveedor tipo);

        [OperationContract]
        MaterialDto BuscarMaterial(int centroId, string criteria);

        [OperationContract]
        MaterialDto BuscarMaterialTodosLosCentros(string criteria);

        [OperationContract]
        string BuscarDescripcionCortaMaterial(int centroId, string criteria);

        [OperationContract]
        IList<MaterialDto> BuscarMaterialesTodosLosCentros(string criteria);

        [OperationContract]
        VinedoDto BuscarVinedo(string criteria);

        [OperationContract]
        IList<VinedoDto> BuscarVinedos(string criteria);

        [OperationContract]
        MaterialPorCentroDto BuscarMaterialPorCentro(int centroId, string criteria);

        [OperationContract]
        ListaPaginada<ProveedorDto> ListarPaginadoProveedores(string filtro, Paginacion paginacion);

        [OperationContract]
        IList<ClienteDto> BuscarClientes(string criteria);

        [OperationContract]
        ClienteDto BuscarCliente(string criteria);

        [OperationContract]
        IList<ClienteDto> BuscarClientesPorCuit(string cuit);

        [OperationContract]
        CartaPorteDto ObtenerCartaPorte(int id);

        [OperationContract]
        CartaPorteDto ObtenerCartaPortePorCentroYNumero(string numero, int centroId);

        [OperationContract]
        CartaPorteResponseDto ObtenerCartaPorteAReutilizarPorNumero(string numero, int centroId, string workflowCodigo);

        [OperationContract]
        CartaPorteResponseDto ObtenerCartaPorteRedespachoPorNumero(string numero, int centroId, string workflowCodigo, int tipoVehiculo, bool cpe, bool consultactg = false);

        [OperationContract]
        CartaPorteDto ObtenerCartaPorteVacia(int centroId, string workflowCodigo, string destinatarioCodigoSap = "", string titularCodigoSap = "", string centroDestino = "", string rtteComercial = "");

        [OperationContract]
        CartaPorteDto ObtenerCartaPorteVaciaFason(int centroId, string workflowCodigo, string destinatarioCodigoSap = "", string titularCodigoSap = "");

        [OperationContract]
        CartaPorteValidaResponseDto NumeroCartaPorteValido(string numero, int centroId, string workflowCodigo, bool CPE = false);

        [OperationContract]
        CartaPorteValidaResponseDto NumeroCartaPorteValidoRedespacho(string numero, int centroId, string workflowCodigo);

        [OperationContract]
        bool ProcedenciaYCodigoValido(int centroId, string codEstablecimiento, int localidadId);

        [OperationContract]
        OrdenDeDescargaDto ObtenerOrdenDeDescarga(int id);

        [OperationContract]
        RemitoDto ObtenerRemito(int id);

        [OperationContract]
        RemitoDto ObtenerRemitoPorNroRemito(string numero);

        [OperationContract]
        RemitoDto ObtenerRemitoPorOrdenDeDescarga(string numero);

        [OperationContract]
        OrdenDeDescargaFasonDto ObtenerOrdenDeDescargaFason(int id);

        [OperationContract]
        OrdenDeDescargaDto ObtenerOrdenDeDescargaPorNumeroDeOrden(string numero);

        [OperationContract]
        OrdenEntrePlantasDto ObtenerOrdenEntrePlantasPorNumeroDeOrden(string numero);

        [OperationContract]
        OrdenDeDescargaFasonDto ObtenerOrdenDeDescargaFasonPorNumeroDeOrden(string numero);

        [OperationContract]
        OrdenDeCargaContenedorDto ObtenerOrdenDeCargaContenedorPorNumeroDeOrden(string numero);

        [OperationContract]
        OrdenDeDescargaFasonDto ObtenerOrdenDeDescargaFasonPorNumeroDeOrdenYCliente(string numero, int clienteId);

        [OperationContract]
        OrdenDeDescargaFasonDto ObtenerOrdenDeDescargaFasonPorNumeroRemito(string numeroRemito);

        [OperationContract]
        HojaDeRutaDto ObtenerHojaDeRutaPorNumero(string numero);

        [OperationContract]
        UsuarioDto ObtenerUsuario(int id);

        [OperationContract]
        UsuarioDto ObtenerUsuarioId(string usuario);

        [OperationContract]
        UsuarioMatriculaDto ObtenerUsuarioMatricula(string usuario);

        [OperationContract]
        IList<UsuarioDto> ListarUsuarios();

        [OperationContract]
        ListaPaginada<UsuarioDto> ListarPaginadoUsuarios(string filtro, Paginacion paginacion);

        [OperationContract]
        ListaPaginada<AnalisisObligatorioDto> ListarPaginadoAnalisisObligatorios(string filtro, Paginacion paginacion);

        [OperationContract]
        AnalisisObligatorioDto ObtenerAnalisisObligatorio(int id);

        [OperationContract]
        IList<WorkflowInfoDto> ListarWorkflowsPorUsuarioYCentro(string nombreUsuario, int centroId);

        [OperationContract]
        TicketAccesoAfipDto ObtenerTicketAccesoAfip();

        [OperationContract]
        SuplenciaDto ObtenerSuplencia(int id);

        [OperationContract]
        DatosDeWorkflowsDto ListarRecorridosEnPlayaExternaPorCentro(FiltroListaDeWorkflowsDto filtro, Paginacion paginacion);

        [OperationContract]
        ListaPaginada<InstanciaWorkflowDto> ListarRecorridosRechazados(FiltroListaDeWorkflowsDto filtro, Paginacion paginacion);

        [OperationContract]
        ListaPaginada<SuplenciaDto> ListarPaginadoSuplencias(string filtro, Paginacion paginacion);

        [OperationContract]
        IList<DescuentoDto> ListarDescuentos(int caracteristica);

        [OperationContract]
        ListaPaginada<EntregadorDto> ListarEntregadores(string filtro, Paginacion paginacion);

        [OperationContract]
        IList<EntregadorDto> BuscarEntregadores(string criteria);

        [OperationContract]
        EntregadorDto BuscarEntregador(string criteria);

        [OperationContract]
        EntregadorDto ObtenerEntregador(int id);

        [OperationContract]
        IList<PaisDto> ListarPaises();

        [OperationContract]
        IList<ProvinciaDto> ListarProvinciasPorPais(int paisId);

        [OperationContract]
        int ObtenerNumeroDocumentoGenerado();

        [OperationContract]
        int ObtenerNumeroMuestraAuditoriaGenerado();

        [OperationContract]
        int ObtenerNumeroDeTicketGenerado(int puestoDeTrabajoId, bool pagoConMercadoPago);

        [OperationContract]
        int ObtenerNumeroDocumentoFasonGenerado();

        [OperationContract]
        int ObtenerNumeroInformeGenerado();

        [OperationContract]
        string ObtenerNumeroOrdenDeDescargaGenerado(int centroId);

        [OperationContract]
        string ObtenerNumeroOrdenEntrePlantasGenerado(int centroId);

        [OperationContract]
        string ObtenerNumeroOrdenDeDescargaFasonGenerado(int centroId);

        [OperationContract]
        int ObtenerNumeroHojaDeRutaGenerado();

        [OperationContract]
        int ObtenerSecuenciaEnvioACamara();

        [OperationContract]
        int ObtenerNumeroRemitoGenerado();

        [OperationContract]
        string ObtenerNumeroOrdenDeCargaContenedorGenerado(int centroId);

        [OperationContract]
        MuestraEnvioACamaraDto ObtenerUltimaMuestraEnvioACamaraPorCaladoId(int caladoId);

        [OperationContract]
        ListaPaginada<MuestraEnvioACamaraDto> ListarMuestrasPorLote(int id, Paginacion paginacion);

        [OperationContract]
        string ObtenerNumeroLote(int loteId);

        [OperationContract]
        string ObtenerNumeroLoteBiotecnologia(int loteId);

        [OperationContract]
        string ObtenerNumeroLoteAuditoria(int loteId);

        [OperationContract]
        string ObtenerArchivoDeMovimientos(int loteId);

        [OperationContract]
        ListaPaginada<ExcepcionEnvioCamaraDto> ListarExcepcionesEnvioCamara(Paginacion paginacion);

        [OperationContract]
        ListaPaginada<AnalisisPorCaracteristicaDto> ListarPaginadoAnalisisYCaladoPorCaracteristica(Guid workflowInstanceId, Paginacion paginacion);

        [OperationContract]
        AnalisisDeCalidadDto ObtenerAnalisisDeCalidadPorInstanceId(Guid workflowInstanceId);

        [OperationContract]
        AnalisisDeCalidadDto ObtenerAnalisisDeCalidadPorCaladoId(int caladoId);

        [OperationContract]
        CartaPorteDto ObtenerCartaPortePorInstanceId(Guid instanceId);

        [OperationContract]
        ListaPaginada<TransaccionSAPDto> ListarPaginadoTransaccionesSAPPorCentro(string filtro, Paginacion paginacion, int centroId);

        [OperationContract]
        ListaPaginada<TransaccionSAPDto> ListarPaginadoTransaccionesSAPFiltradas(int material, int centro, int? tipoComercial, FuncionSAP funcionSap, Paginacion paginacion);

        [OperationContract]
        ListaPaginada<TransmisionASapDto> ListarTransmisionesASap(FiltroPanelDeTransaccionesSapDto filtro, Paginacion paginacion);

        [OperationContract]
        ListaPaginada<TransmisionBajaCtgDefinitivaDto> ListarRetransmisionCtgDefinitiva(FiltroPanelDeBajaCtgDefinitivaDto filtro, Paginacion paginacion);

        [OperationContract]
        ListaPaginada<EnvioUrenportDto> ListarEnvioUrenport(FiltroPanelDeBajaCtgDefinitivaDto filtro, Paginacion paginacion);

        [OperationContract]
        IList<AlmacenDto> ListarAlmacenesPorMaterial(int centroId, int materialId);

        [OperationContract]
        IList<AlmacenDto> ListarAlmacenesPorMaterialFiltrado(int materialId);

        [OperationContract]
        IList<AlmacenDto> ListarAlmacenesPorMaterialSinFiltro(int materialId);

        [OperationContract]
        IList<AlmacenDto> ListarAlmacenesNoGranos();

        [OperationContract]
        TransmisionASapDto ObtenerTransmisionASap(int id);

        [OperationContract]
        IList<TransmisionesCupoEnErrorDto> ListarCuposEnErrorASapPorFiltro(FiltroPanelDeTransaccionesSapDto filtro);

        [OperationContract]
        IList<int> CaladoPorCaracteristicaQueSeEnvianACamara(int caladoId);

        [OperationContract]
        IList<ControlRecorridoDto> ListarControlRecorridoServiciosSap(Guid workflowId);

        [OperationContract]
        ControlRecorridoDto ObtenerControlRecorrido(Guid workflowId, string actividad);

        [OperationContract]
        ListaPaginada<MaterialPorWorkflowDto> ListarPaginadoMaterialPorWorkflow(string filtro, Paginacion paginacion, int centroId);

        [OperationContract]
        ListaPaginada<CaracteristicaDeCalidadPorWorkflowDto> ListarPaginadoCaracteristicaDeCalidadPorWorkflow(string filtro, Paginacion paginacion, int centroId);

        [OperationContract]
        MaterialPorWorkflowDto ObtenerMaterialPorWorkflow(int id);

        [OperationContract]
        CaracteristicaDeCalidadPorWorkflowDto ObtenerCaracteristicaDeCalidadPorWorkflow(int id);

        [OperationContract]
        IList<MaterialPorWorkflowDto> ListarMaterialesPorWorkflow(int workflowId, int centroId);

        [OperationContract]
        IList<MaterialPorWorkflowDto> ListarMaterialesPorWorkflowYVinedo(int workflowId, int centroId, int vinedoId);

        [OperationContract]
        IList<TipoBinDto> ListarMaterialesBin(int workflowId, int centroId, ClaseBin? claseBin);

        [OperationContract]
        IList<MaterialDto> ListarMaterialesBinPallet();

        [OperationContract]
        IList<MaterialPorWorkflowDto> BuscarMaterialesPorWorkflow(int workflowId, int centroId, string filtro);

        [OperationContract]
        MuestraEnvioACamaraDto ObtenerMuestraEnvioACamaraPorNumero(int centroId, string nroMuestra);

        [OperationContract]
        MuestraEnvioACamaraYRecorridoDto ObtenerMuestraEnvioACamaraYRecorridoPorNumero(string nroMuestra, int centroId, bool incluirPreLote);

        [OperationContract]
        string ObtenerNumeroMuestraEnvioACamara(int muestraId);

        [OperationContract]
        MuestraEnvioACamaraDto ObtenerMuestraEnvioACamaraPorCalado(int caladoId);

        [OperationContract]
        LoteDto ObtenerLoteParaArchivo(int id);

        [OperationContract]
        IEnumerable<string> ObtenerGruposPorUsuario(string nombreUsuario);

        [OperationContract]
        NotificacionesDto ObtenerNotificaciones(string grupos, bool listarSobre, bool mostrarAlerta, bool contar);

        [OperationContract]
        IList<MuestraEnvioACamaraDto> ListarMuestraEnvioACamaraSinLote(int centroId, bool incluirPreLote);

        [OperationContract]
        IList<MuestraEnvioACamaraBiotecnoligiaDto> ListarMuestraEnvioACamaraBiotecnologiaSinLote(int materialId, int camaraId, int centroId);

        [OperationContract]
        List<MovimientoDeTercerosListaDto> ListarMovimientoDeTercerosSinArchivo(int materialId, TipoDeWorkflow tipoDeWorkflow, int centroId);

        [OperationContract]
        IList<LoteDto> ListarLotesPorWorkflow(Guid workflowInstance);

        [OperationContract]
        IList<CalleDto> ListarCalles(int centroId);

        [OperationContract]
        IList<PuestosDeCargaDescargaDto> ListarHidraulicas(int centroId, bool esSustentable);

        [OperationContract]
        CalleDto ObtenerCalle(int id);

        [OperationContract]
        string ObtenerCalleNombre(int id);

        [OperationContract]
        PuestosDeCargaDescargaDto ObtenerHidraulica(int id);

        [OperationContract]
        string ObtenerHidraulicaNombre(int id);

        [OperationContract]
        ListaPaginada<ConversionMaterialDto> ListarPaginadoConversionMaterial(int? camaraId, Paginacion paginacion);

        [OperationContract]
        ListaPaginada<ConversionCentroDto> ListarPaginadoConversionCentro(int? camaraId, Paginacion paginacion);

        [OperationContract]
        ConversionMaterialDto ObtenerConversionMaterial(int camaraId, int materialId);

        [OperationContract]
        ListaPaginada<ConversionProcedenciaDto> ListarPaginadoConversionProcedencia(Paginacion paginacion, int? camaraId);

        [OperationContract]
        ListaPaginada<ConversionGrupoDto> ListarPaginadoConversionGrupo(Paginacion paginacion, int? camaraId);

        [OperationContract]
        ListaPaginada<ConversionCaracteristicaDto> ListarPaginadoConversionCaracteristica(Paginacion paginacion, int? camaraId, int? materialId);

        [OperationContract]
        ConversionGrupoDto ObtenerConversionGrupo(int camaraId, int materialId);

        [OperationContract]
        ConversionCaracteristicaDto ObtenerConversionCaracteristica(int camaraId, int caracteristicaId);

        [OperationContract]
        IList<PermisoDto> ListarPermisosPorActividad(string actividad);

        [OperationContract]
        IList<AnalisisPorCaracteristicaDto> ListarAnalisisYCaladoPorCaracteristica(Guid workflowInstanceId);

        [OperationContract]
        IList<AnalisisPorCaracteristicaDto> ListarAnalisisYCaladoPorCaracteristicaNoAceptables(Guid workflowInstanceId);

        [OperationContract]
        IList<AnalisisPorCaracteristicaDto> ListarAnalisisYCaladoPorCaracteristicaConAdvertencia(Guid instanciaWorkflow);

        [OperationContract]
        IList<AnalisisPorCaracteristicaDto> ListarAnalisisYCaladoPorId(int caladoId);

        [OperationContract]
        RomaneoDto ObtenerUltimoRomaneoPorGuid(Guid guid);

        [OperationContract]
        IList<RomaneoDto> ObtenerRomaneosPorGuid(Guid guid);

        [OperationContract]
        RomaneoDto ObtenerRomaneoPorNroPedido(string NroPedido);

        [OperationContract]
        IList<RomaneoItemDto> ObtenerItemRomaneosPorRomaneoId(int romaneoId);

        [OperationContract]
        RomaneoDto ObtenerRomaneo(int id);

        [OperationContract]
        RomaneoDto ObtenerRomaneoProveedor(int id);

        [OperationContract]
        OrdenDeDescargaDto ObtenerOrdenDeDescargaPorInstanceId(Guid instanceId);

        [OperationContract]
        OrdenDeDescargaFasonDto ObtenerOrdenDeDescargaFasonPorInstanceId(Guid instanceId);

        [OperationContract]
        OrdenDeDescargaDto ObtenerOrdenDeDescargaPorNumero(string numero);

        [OperationContract]
        OrdenDeDescargaFasonDto ObtenerOrdenDeDescargaFasonPorNumero(string numero);

        [OperationContract]
        OrdenCargaFasDto ObtenerOrdenCargaFas(int id);

        [OperationContract]
        OrdenCargaFasDto ObtenerOrdenCargaFasPorInstanceId(Guid instanceId);

        [OperationContract]
        ProveedorDto ObtenerProveedor(int id);

        [OperationContract]
        ProveedorDto ObtenerProveedorPorCuit(string cuit, TiposProveedor tipoProveedor);

        [OperationContract]
        DocumentoDeImpresionPorCentroDto ObtenerDocumentoDeImpresionPorCentro(int id);

        [OperationContract]
        string ObtenerProveedorPorNroPedidoEnRomaneo(string numero, Guid instanceId);

        [OperationContract]
        string ObtenerProveedorPorNroPedidoEnDescargaUnidad(string numero, Guid instanceId);

        [OperationContract]
        decimal ObtenerPesoNetoRomaneo(Guid instanceId);

        [OperationContract]
        IList<AjusteDeCalidadDto> ListarCaracteristicasParaAjustesDeCalidad(int caladoId);

        [OperationContract]
        ListaPaginada<ImpresionDto> ListarImpresiones(TipoDocumentoIngreso? tipo, string numeroDocumentoIngreso, string patente, TipoImpresion? tipoImpresion, Paginacion paginacion,int centroId);

        [OperationContract]
        VehiculoDto ObtenerVehiculoPorGuid(Guid instanceId);

        [OperationContract]
        IList<CartaPorteDto> ListarCartasDePortePorCentroYFecha(int centroId, DateTime fechaInicio, DateTime fechaFin, TipoDeWorkflow tipoDeWorkflow);

        [OperationContract]
        IList<OnccaEmitidasDto> ListarArchivoOncca(List<int> centros, DateTime fechaInicio, DateTime fechaFin, TipoDeWorkflow tipoDeWorkflow);

        [OperationContract]
        IList<ListadoCamionesDto> ListarCartasDePortePorCentroFechaTiposComercialesYMaterial(List<int> centros, DateTime fechaInicio, DateTime fechaFin, List<int> tiposComerciales, List<int> materiales, bool incluirRechazados);

        [OperationContract]
        IList<FormatoDePapelDto> ListarFormatoDePapel();

        [OperationContract]
        IList<ImpresoraDto> ListarImpresoras(int centroId);

        [OperationContract]
        IList<BajaCTGRetransmisionDto> ObtenerBajasCtgDefinitivas(int[] ids);

        [OperationContract]
        IList<EnvioUrenportDto> ObtenerEnvioUrenports(int[] ids);

        [OperationContract]
        ListaPaginada<ImpresoraDto> ListarPaginadoImpresoras(int centroId, string filtro, Paginacion paginacion);

        [OperationContract]
        ListaPaginada<FormatoDeImpresionDto> ListarPaginadoFormatosDeImpresion(string filtro, Paginacion paginacion);

        [OperationContract]
        IList<FormatoDeImpresionDto> ListarFormatosDeImpresion();

        [OperationContract]
        ListaPaginada<DocumentoDeImpresionPorCentroDto> ListarPaginadoDocumentoDeImpresionPorCentro(Paginacion paginacion, string filtro, int centrosId);

        [OperationContract]
        IList<DocumentoDeImpresionPorCentroDto> ListarDocumentoDeImpresion(int documentoImpresion, int? formatoImpresion);

        [OperationContract]
        IList<DocumentoDeImpresionDto> ListarDocumentosDeImpresion();

        [OperationContract]
        IList<FormatoDePapelDto> ListarFormatosDePapel();

        [OperationContract]
        FormatoDeImpresionDto ObtenerFormatoDeImpresion(int id);

        [OperationContract]
        IList<CampoDto> ListarCampos();

        [OperationContract]
        IList<LetraDto> ListarLetras();

        [OperationContract]
        IList<FormatoDeCampoDto> ListarFormatosDeCampo(int id);

        [OperationContract]
        ImpresoraDto ObtenerImpresora(int id);

        [OperationContract]
        DocumentoDeImpresionPorCentroDto ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(string codigo, int centroId, int puestoDeTrabajoId);

        [OperationContract]
        bool ExisteTransaccionSAP(int materialId, int centroId, FuncionSAP funcion, Guid instanceId);

        [OperationContract]
        OrdenEntrePlantasDto ObtenerOrdenEntrePlantas(int id);

        [OperationContract]
        OrdenEntrePlantasDto ObtenerOrdenEntrePlantasPorInstanceId(Guid id);

        [OperationContract]
        HojaDeRutaDto ObtenerHojaDeRuta(int id);

        [OperationContract]
        RemitoDto ObtenerRemitoPorInstanceId(Guid id);

        [OperationContract]
        DescargaUnidadDto ObtenerUltimaDescargaUnidadPorGuid(Guid guid);

        [OperationContract]
        DescargaUnidadDto ObtenerDescargaUnidadPorNroPedido(string nroPedido);

        [OperationContract]
        DescargaUnidadDto ObtenerDescargaUnidadProveedor(int id);

        [OperationContract]
        IList<DescargaUnidadDto> ObtenerDescargaUnidadPorGuid(Guid guid);

        [OperationContract]
        DescargaUnidadDto ObtenerDescargaUnidad(int id);

        [OperationContract]
        bool ValidacionCtgEsAutomatica(int centroId);

        [OperationContract]
        bool ValidacionCtgEsManual(int centroId);

        [OperationContract]
        decimal ObtenerPesoNetoDescargaUnidad(Guid instanceId);

        [OperationContract]
        OrdenDeCargaContenedorDto ObtenerOrdenDeCargaContenedor(int id);

        [OperationContract]
        OrdenDeCargaContenedorDto ObtenerOrdenDeCargaContenedorPorInstanceId(Guid instanceId);

        [OperationContract]
        ListaPaginada<TaraContenedorDto> ListarPaginadoTaraContenedor(string filtro, Paginacion paginacion);

        [OperationContract]
        TaraContenedorDto ObtenerTaraContenedor(int id);

        [OperationContract]
        IList<TaraContenedorDto> ListarTaraContenedores();

        [OperationContract]
        ListaPaginada<CalleDto> ListarPaginadoCalle(int centroId, Paginacion paginacion);

        [OperationContract]
        ListaPaginada<CalidadMaterialDto> ListarPaginadoCalidadMaterial(int materialId, int centroId, Paginacion paginacion);

        [OperationContract]
        ListaPaginada<TarjetaBloqueadaDto> ListarPaginadoTarjetasBloqueadas(string filtro, Paginacion paginacion, int centroId);

        [OperationContract]
        TarjetaBloqueadaDto ObtenerTarjetaBloqueada(int id);

        [OperationContract]
        ReciboMunicipalDto ObtenerReciboMunicipal(int centroId);

        [OperationContract]
        ListaPaginada<TarjetaRangoDto> ListarPaginadoTarjetasRango(string filtro, Paginacion paginacion, int centroId);

        [OperationContract]
        TarjetaRangoDto ObtenerTarjetaRango(int id);

        [OperationContract]
        ListaPaginada<TarjetaSupervisorDto> ListarPaginadoTarjetasSupervisor(string filtro, Paginacion paginacion, int centroId);

        [OperationContract]
        TarjetaSupervisorDto ObtenerTarjetaSupervisor(int id);

        [OperationContract]
        ListaPaginada<PuestosDeCargaDescargaDto> ListarPaginadoHidraulica(int centroId, Paginacion paginacion);

        [OperationContract]
        IList<LectorDto> ListarLectores();

        [OperationContract]
        PuestoDeTrabajoDto ObtenerPuestoDeTrabajo(int id);

        [OperationContract]
        PuestoDeTrabajoDto ObtenerPuestoDeTrabajoPorNombrePc(string nombrePc, int centroId);

        [OperationContract]
        bool RedireccionarAListaAutomatizada(string nombrePc, int centroId);

        [OperationContract]
        bool RedireccionarABalanzaAutomatizada(string nombrePc, int centroId);

        [OperationContract]
        IList<VagonDto> ListarVagonesEnPesada(int centroId);

        [OperationContract]
        PuestoDeTrabajoDto ObtenerPuestoDeTrabajoPorDispositivo(string codigoDispositivo);

        [OperationContract]
        IList<PuestoDeTrabajoDto> ListarPuestosDeTrabajo();

        [OperationContract]
        ListaPaginada<PuestoDeTrabajoDto> ListarPaginadoPuestosDeTrabajo(string filtro, int centroId, Paginacion paginacion);

        [OperationContract]
        ListaPaginada<PuestoDeTrabajoContingenciaDto> ListarPaginadoPuestosDeTrabajoContingencia(string filtro, int centroId, Paginacion paginacion);

        [OperationContract]
        int ObtenerNumeroAleatorio();

        [OperationContract]
        AlmacenDto ObtenerAlmacenPredeterminado(int centroId, int materialId);

        [OperationContract]
        IList<ExportacionDeArchivosSelectObjDto> ListarTiposComercialesConFiltro(int cantResultados, int pagina);

        [OperationContract]
        IList<ExportacionDeArchivosSelectObjDto> ListarCentrosConFiltro(int cantResultados, int pagina);

        [OperationContract]
        IList<ListadoDeCalidadesDto> ListarListadoDeCalidades(List<int> centros, DateTime fechaInicio, DateTime fechaFin, List<int> tiposComerciales, int material);

        [OperationContract]
        IList<MuestraDeHumedadDto> ListarListadoDeMuestrasDeHumedad(int? centro, int? humedimetro, DateTime fechaDesde, DateTime fechaHasta);

        [OperationContract]
        ListaPaginada<ControlDeTiempoDto> ListarPaginadoControlesDeTiempo(Paginacion paginacion, int centroId);

        [OperationContract]
        ControlDeTiempoDto ObtenerControlDeTiempoPorCodigoControlPorGuid(string codigoControl, Guid instanceId);

        [OperationContract]
        LogActividadDto ObtenerUltimoLogActividad(Guid instanceId, string actividad);

        [OperationContract]
        LogActividadDto ObtenerUltimoLog(Guid instanceId);

        [OperationContract]
        AutorizacionTiempoEnTransitoDto ObtenerAutorizacionTiempoEnTransito(Guid instanceId, string codigoControl, string nombreUsuario);

        [OperationContract]
        ControlDeTiempoDto ObtenerControlDeTiempo(int id);

        [OperationContract]
        ActividadPorDispositivoDto ObtenerActividadPorDispositivo(int id);

        [OperationContract]
        ListaPaginada<ActividadPorDispositivoDto> ListarPaginadoActividadesPorBarreraSemaforo(int puestoId, Paginacion paginacion);

        [OperationContract]
        bool EsTarjetaBloqueada(string numero, int centroId);

        [OperationContract]
        bool EsTarjetaEnRangoValido(string numero, int centroId);

        [OperationContract]
        string ObtenerTarjetaRFIDAsignada(TipoDocumentoIngreso tipoDocumentoIngreso, string nroDocumentoIngreso);

        [OperationContract]
        Guid ObtenerInstanceIdPorTipoYNumero(TipoDocumentoIngreso tipoDocumentoIngreso, string nroDocumentoIngreso);

        [OperationContract]
        ListaPaginada<MotivoQuiebreBarreraDto> ListarPaginadoMotivoQuiebreBarrera(int puestoId, int centroId, Paginacion paginacion);

        [OperationContract]
        MotivoQuiebreBarreraDto ObtenerMotivoQuiebreBarrera(int id);

        [OperationContract]
        IList<PuestoDeTrabajoDto> ListarPuestosDeTrabajoPorCentro(int centroId);

        [OperationContract]
        IList<PuestoDeTrabajoDto> ListarPuestosDeTrabajoPorNombrePc(string nombrePc, int centroId);

        [OperationContract]
        IList<string> ListarCamarasPorNombrePc(string nombrePc, int centroId);

        [OperationContract]
        AsignacionDto ObtenerAsignacionDePuestoComando(string instanceId);

        [OperationContract]
        IList<CalidadMaterialDto> ListarCalidadesPorCentro(int centroId);

        [OperationContract]
        bool BalanzasObligatoriasEnPuestoComando(int centroId);

        [OperationContract]
        DatosDeWorkflowsDto ListarWorkFlows(Paginacion paginacion, FiltroListaDeWorkflowsDto filtro);

        [OperationContract]
        CalidadMaterialDto ObtenerCalidadMaterial(int id);

        [OperationContract]
        CalidadMaterialDto ObtenerCalidadMaterialPorHumedadEInstanceId(decimal valorHumedad, bool piedeAnalisis, Guid instancieId);

        [OperationContract]
        Guid ObtenerRecorridoInstanceIdPorTarjetaDeAcceso(string numero, int centroId);

        [OperationContract]
        int ObtenerNumeroControlDeCargaGenerado();

        [OperationContract]
        bool WorkflowEstaAsignado(Guid instanceId);

        [OperationContract]
        IList<string> ListarCaracteristicaConfiguracionDeTabla(int centroId, int materialId, string usuario);

        [OperationContract]
        IList<CaracteristicaConfiguracionDeTablaDto> ListarCaracteristicasDeCalidadPorConfiguracion(int centroId, int materialId, string nombreUsuario);

        [OperationContract]
        ControlDeBalanzaDto ObtenerControlDeBalanza(int id);

        [OperationContract]
        ControlDeBalanzaDto ObtenerControlDeBalanzaPorRecorrido(int recorridoId);

        [OperationContract]
        ListaPaginada<AjusteDeStockDto> ListarPaginadoAjusteDeStock(string filtro, Paginacion paginacion, int centroId);

        [OperationContract]
        ListaPaginada<AjusteStockBinesDto> ListarPaginadoAjusteYStockBines(string filtro, Paginacion paginacion, int centroId);

        [OperationContract]
        ListaPaginada<MovimientoDeBinesDto> ListarPaginadoMovimientoDeBines(string filtro, Paginacion paginacion, int centroId);

        [OperationContract]
        AjusteDeStockDto ObtenerAjusteDeStock(int id);

        [OperationContract]
        AjusteStockBinesDto ObtenerAjusteYStockBines(int id);

        [OperationContract]
        IList<ControlDeBalanzaPesadaDto> ListarControlDeBalanza(DateTime desde, DateTime hasta);

        [OperationContract]
        DatosRecorridoDto ObtenerDatosRecorridoActivo(string patente, IList<string> lecturasTarjetaDeAcceso);

        [OperationContract]
        DatosRecorridoDto ObtenerDatosRecorridoActivoSinTarjeta(string patente, string lecturasTarjetaDeAcceso);

        [OperationContract]
        DatosRecorridoDto ObtenerDatosRecorridoActivoPorWorkflow(Guid workflow);

        [OperationContract]
        ValidarProximaAccionPorPuestoDto ValidarProximaActividadPorPuesto(DatosRecorridoDto recorrido, string proximaActividad, IList<PuestoDeTrabajoDto> puestos, string nombreUsuario);

        [OperationContract]
        ValidarProximaAccionDto ValidarProximaActividadPorPuestoSinPatente(DatosRecorridoDto recorrido, string proximaActividad, IList<PuestoDeTrabajoDto> puestos);

        [OperationContract]
        IList<ListadoDePesadasDto> ListarListadoDePesadas(List<int> centros, DateTime fechaInicio, DateTime fechaFin, List<int> tiposComerciales, List<int> materiales, bool incluirRechazados);

        [OperationContract]
        VerificarKilosDeclaradosPorFincaDto VerificarKilosDeclaradosPorVinedo(int vinedoId, int materialId, string cosecha);

        [OperationContract]
        decimal ObtenerKilosRecibidosPorVinedo(int vinedoId, int variedadId, string cosecha);

        [OperationContract]
        VerificarKilosDeclaradosPorFincaDto ObtenerKilosARecibirPorVinedo(int vinedoId, int variedadId, string cosecha);

        [OperationContract]
        ListaPaginada<CiuAnuladoDto> ListarPaginadoCiuAnulados(Paginacion paginacion);

        [OperationContract]
        ListaPaginada<VinedoPropioDto> ListarVinedoPropio(string filtro, Paginacion paginacion);

        [OperationContract]
        IList<VinedoPropioDto> ListarVinedosPropios();

        [OperationContract]
        IList<VinedoTercerosDto> ListarVinedosTerceros(int proveedorId);

        [OperationContract]
        bool EsCiuAnulado(string numeroCiu);

        [OperationContract]
        RemitoBodegaUvaDto ObtenerRemitoBodegaUvaPorGuid(Guid guid);

        [OperationContract]
        int ObtenerRemitoBodegaUvaIdPorGuid(Guid guid);

        [OperationContract]
        IList<VariedadDto> ListarVariedades();

        [OperationContract]
        RemitoBodegaUvaDto ObtenerRemitoBodegaUva(int id);

        [OperationContract]
        RemitoBodegaUvaDto ObtenerRemitoBodegaUvaPorInstanceId(Guid instanceId);

        [OperationContract]
        RemitoBodegaVinoDto ObtenerRemitoBodegaVino(int id);

        [OperationContract]
        RemitoBodegaVinoDto ObtenerRemitoBodegaVinoPorInstanceId(Guid instanceId);

        [OperationContract]
        HojaDeRutaDto ObtenerHojaDeRutaPorInstanceId(Guid instanceId);

        [OperationContract]
        IList<CuartelDto> FiltrarCuartelesPorVinedo(int vinedoId);

        [OperationContract]
        IList<DescargaDeBinesDto> ObtenerDescargasDeBinesPorRemitoBodegaUva(int remitoBodegaUvaId);

        [OperationContract]
        VinedoPropioDto ObtenerVinedoPropio(int id);

        [OperationContract]
        VinedoTercerosDto ObtenerVinedoTerceros(int id);

        [OperationContract]
        DistribucionDeAlmacenesDto ObtenerDistribucionDeAlmacenes(Guid guid);

        [OperationContract]
        ListaPaginada<VinedoTercerosDto> ListarVinedoTerceros(string filtro, Paginacion paginacion);

        [OperationContract]
        IList<MaterialPorWorkflowDto> ListarMaterialesPorWorkflowVinedoYCodigoSAP(int workflowId, int centroId, int vinedoId, List<string> materialesCodigoSap);

        [OperationContract]
        IList<MaterialPorWorkflowDto> ListarMaterialesPorWorkflowYCodigoSAP(int workflowId, int centroId, List<string> materialesCodigoSap);

        [OperationContract]
        VariedadDto ObtenerVariedadPorMaterial(int id);

        [OperationContract]
        decimal? ObtenerTenorAzucarinoNumerico(Guid guid);

        [OperationContract]
        string ObtenerTenorAzucarino(Guid guid);

        [OperationContract]
        ListaPaginada<AsignacionDeRecorridoDto> ListarPaginadoAsignacionesDeRecorrido(int centroId, string filtro, Paginacion paginacion);

        [OperationContract]
        AsignacionDeRecorridoDto ObtenerAsignacionDeRecorrido(int id);

        [OperationContract]
        IList<MaterialPorCentroDto> ListarMaterialesPorCentro(int centroId);

        [OperationContract]
        IList<CalidadMaterialDto> ListarCalidadesPorMaterialyCentro(int materialPorCentroId);

        [OperationContract]
        HojaDeRutaYerbateraDto ObtenerHojaDeRutaYerbatera(int id);

        [OperationContract]
        HojaDeRutaYerbateraDto ObtenerHojaDeRutaYerbateraPorInstanceId(Guid id);

        [OperationContract]
        HojaDeRutaYerbateraDto ObtenerHojaDeRutaYerbateraVacia(int centroId, string codigoWorkflow, string codigoDestinatario, string codigoProveedor);

        [OperationContract]
        HojaDeRutaYerbateraValidaResponseDto NumeroHojaDeRutaYerbateraValido(string numero, int centroId, string workflow);

        [OperationContract]
        AsignacionDeRecorridoDto BuscarAsignacionDeRecorridoPorInstanceId(Guid instanceId);

        [OperationContract]
        IList<ArchivoINVFilaDto> ListarArchivoINV(DateTime fechaDesde, DateTime fechaHasta, int centroId);

        [OperationContract]
        bool EsActividadAutomatica(Guid guid, string nombreActividad, string codigo);

        [OperationContract]
        ListaPaginada<ActividadConCargaAutomaticaDto> ListarPaginadoActividadesConCargaAutomatica(int centroId, string filtro, Paginacion paginacion);

        [OperationContract]
        ActividadConCargaAutomaticaDto ObtenerActividadConCargaAutomatica(int id);

        [OperationContract]
        bool ActividadEsEjecutable(string workflowCodigo, string actividad, string nombreUsuario);

        [OperationContract]
        IList<string> ListarPermisosDeActividadPorUsuario(string nombreUsuario);

        [OperationContract]
        bool UsuarioTienePermisoParaActividad(string nombreUsuario, string actividad);

        [OperationContract]
        IList<string> ListarPermisosDeActividad();

        [OperationContract]
        int LeerPesoMaximo(Guid guid, TipoDeWorkflow tipoDeWorkflow);

        [OperationContract]
        int LeerToleranciaRechazo(Guid guid);

        [OperationContract]
        int ObtenerToleranciaRomaneo(Guid guid);

        [OperationContract]
        int ObtenerToleranciaOrigen(Guid guid);

        [OperationContract]
        bool VerificarCorrespondeControlDeBalanza(Guid guid);

        [OperationContract]
        bool VerificarCorrespondeDescarga(Guid guid);

        [OperationContract]
        BalanzasDto ObtenerBalanzasPorGuid(Guid guid);

        [OperationContract]
        Formulario239RecorridoMaterialCentroDto ObtenerRecorridoMaterialCentroImpresionFormulario239(Guid guid);

        [OperationContract]
        ImpresionReciboMunicipalRecorridoDto ObtenerRecorridoImpresionReciboMunicipal(Guid guid);

        [OperationContract]
        ImpresionCartaPorteUrenportRecorridoDto ObtenerRecorridoCartaPorteUrenport(int cartaporteId);

        [OperationContract]
        EstablecimientoDto ObtenerEstablecimiento(int id);

        [OperationContract]
        ListaPaginada<EstablecimientoDto> ListarPaginadoEstablecimientos(string filtro, Paginacion paginacion);

        [OperationContract]
        bool EsProveedorSustentable(int proveedorId);

        [OperationContract]
        AsignacionDeEstablecimientoDto ObtenerAsignacionDeEstablecimiento(Guid instanceId);

        [OperationContract]
        IList<DatosInstanciaWorkflowDto> ListarDatosDeWorkflows(List<Guid> instanceIds);

        [OperationContract]
        DatosInstanciaWorkflowPuertoDto ListarDatosDeWorkflowsPuerto(List<Guid> instanceIds);

        [OperationContract]
        TimeSpan? ObtenerTiempoEntreActividades(Guid instanceId, string actividadDesde, string actividadHasta);

        [OperationContract]
        IList<int> ObtenerPuestosIdPorPC(string nombrePc);

        [OperationContract]
        bool VerificarPuestodeCargaDescarga(Guid instanceId, int puestoDeTrabajoId);

        [OperationContract]
        bool MaterialImprimeReciboMunicipal(Guid instanceId);

        [OperationContract]
        bool? RecorridoPagaTicketMunicipal(Guid instanceId);

        [OperationContract]
        IList<TecnologiaDto> ListarTecnologias();

        [OperationContract]
        IList<EmpresaDto> ListarEmpresas();

        [OperationContract]
        ListaPaginada<EmpresaDto> ListarPaginadoEmpresa(string filtro, Paginacion paginacion);

        [OperationContract]
        ListaPaginada<TecnologiaDto> ListarPaginadoTecnologia(string filtro, Paginacion paginacion);

        [OperationContract]
        EmpresaDto ObtenerEmpresa(int id);

        [OperationContract]
        TecnologiaDto ObtenerTecnologia(int id);

        [OperationContract]
        bool RequiereTecnologia(Guid instanceId);

        [OperationContract]
        bool CorrespondeRegistrarCartadePorteTren(Guid instanceId);

        [OperationContract]
        int ObtenerCantidadVagones(Guid instanceId);

        [OperationContract]
        IList<AnalisisVagonDto> ObtenerAnalisisPorVagones(Guid instanceId);

        [OperationContract]
        bool CorrespondeRegistrarMuestreoYPesajeTren(Guid instanceId);

        [OperationContract]
        CartaDePorteRegistradaServicioMonsantoDto ObtenerCartaDePorteRegistradaServicioMonsanto(Guid instanceId, TipoVehiculo tipo);

        [OperationContract]
        Guid ObtenerRecorridoInstanceIdPorRecorridoId(int instanceId);

        [OperationContract]
        bool TienePermiso(string nombreUsuario, PermisosScato permiso);

        [OperationContract]
        bool CartaPorteTieneEntregador(int id);

        [OperationContract]
        InfoCaladoDto ObtenerInformacionCartaPorte(int recorridoId);

        [OperationContract]
        HumedimetroDto ObtenerHumedimetroPorNombrePc(int centroId, string nombrePc);

        [OperationContract]
        NirsDto ObtenerNirsPorNombrePc(int centroId, string nombrePc);

        [OperationContract]
        ListaPaginada<ProveedorExcluidoIntactaDto> ListarPaginadoProveedorExcluidoIntacta(string filtro, Paginacion paginacion);

        [OperationContract]
        bool EsProveedorExcluidoIntacta(int id);

        [OperationContract]
        LoteBiotecnologiaDto ObtenerLoteBiotecnologiaParaArchivo(int loteId);

        [OperationContract]
        bool ExistenMuestraEnvioACamaraIntactaPendientes(int materialId, int centroId);

        [OperationContract]
        LoteBiotecnologiaDto ObtenerLoteBiotecnologiaParaImpresion(int loteId);

        [OperationContract]
        ListaPaginada<MuestraEnvioACamaraBiotecnoligiaDto> ListarMuestrasPorLoteBiotecnologia(int loteId, Paginacion paginacion);

        [OperationContract]
        ListaPaginada<MuestraEnvioACamaraAuditoriaDto> ListarMuestrasPorLoteAuditoria(int loteId, Paginacion paginacion);

        [OperationContract]
        LoteAuditoriaDto ObtenerLoteAuditoriaParaArchivo(int loteId);

        [OperationContract]
        LoteAuditoriaDto ObtenerLoteAuditoriaParaImpresion(int loteId);

        [OperationContract]
        ListaPaginada<MovimientoDeTercerosListaDto> ListarMuestrasPorArchivoDeMovimientos(int loteId, Paginacion paginacion);

        [OperationContract]
        ImpresionDto ObtenerUltimaImpresionPorInstanceIdYCodigo(Guid id, string codigo);

        [OperationContract]
        IList<ZonaDto> ListarZonas();

        [OperationContract]
        IList<SubZonaDto> ListarSubZonasPorZona(int zonaId);

        [OperationContract]
        TransmisionSapAModificarDto ObtenerTransmisionASapPorIdyFuncionSap(int id);

        [OperationContract]
        IList<int> ObtenerTransmisionesCuposIdsConError();

        [OperationContract]
        PanelServerAppPoolDto ObtenerEstadoServidor(string nombre);

        [OperationContract]
        void IniciarAppPool(string nombre);

        [OperationContract]
        void DetenerAppPool(string nombre);

        [OperationContract]
        void ReciclarAppPool(string nombre);

        [OperationContract]
        void ConfigurarTiempoReciclado(string nombre, double tiempo);

        [OperationContract]
        IEnumerable<AppPoolDto> ListarAppPools();

        [OperationContract]
        List<EventLogEntry> ObtenerLogEventos(string serverNombre, int idEvento, DateTime fechaDesde, DateTime fechaHasta);

        [OperationContract]
        List<ControlRecorridoLogActividadConsultaDto> ConsultaControlRecorridoLogActividad(Guid id);

        [OperationContract]
        MovimientoDeBinesDto ObtenerMovimientoDeBines(int id);

        [OperationContract]
        int ObtenerStock(TipoStockBines tipo, DateTime fecha, int id, int materialId);

        [OperationContract]
        int LibroMovimientosExistenciaGranosCalcularHojas(LibroMovimientosExistenciaGranosDto dto, int centroId);

        [OperationContract]
        FirmaDto ObtenerFirma();

        [OperationContract]
        bool LibroOnccaEsPorDescripcionCorta(int id);

        [OperationContract]
        List<ServicioDto> ListarEstadoDeServicios();

        [OperationContract]
        DateTime? ObtenerUltimaPesadaFechaPorGuid(Guid id);

        [OperationContract]
        int ObtenerProveedorIdPorRecorrido(Guid workflowInstanceId);

        [OperationContract]
        bool MuestraConjuntoFueUtilizada(Guid workflowInstanceId, int muestraConj);

        [OperationContract]
        DestinatarioCTGDto ObtenerDestinatarioCTGporGuid(Guid workflowInstanceId);

        [OperationContract]
        bool ExistenMovimientosDeTercerosPendientes(int materialId, TipoDeWorkflow tipoDeWorkflow, int centroId);

        [OperationContract]
        ArchivoDeMovimientosDto ObtenerMovimientosDeTercerosParaArchivo(int archivoId);

        [OperationContract]
        GraficoDePlantaDto ObtenerGraficoDePlanta(string nombreActividad, int centroId);

        [OperationContract]
        IList<GraficoDePlantaDto> ListarGraficosDePlanta(int centroId);

        [OperationContract]
        IList<FirmaDto> ListarFirmas();

        [OperationContract]
        IngresoDeDatosDeExportacionDto ObtenerIngresoDeDatosDeExportacionPorRecorrido(int id);

        [OperationContract]
        List<string> ListarCuitfirmas();

        [OperationContract]
        IList<ColaImpresionDto> ConsultarColaImpresion(int id, string servidor);

        [OperationContract]
        void AccionJobImpresion(int impresoraId, int jobId, AccionColaImpresion opcion);

        [OperationContract]
        decimal? ObtenerPesoNetoConDescuento(Guid instanceId);

        [OperationContract]
        decimal? ObtenerPesoNetoSinDescuento(Guid instanceId);

        [OperationContract]
        CamaraDto ObtenerCamaraDeExcepcionDescuento(Guid instanceId, int materialId, int centroId);

        [OperationContract]
        List<CaracteristicaDeCalidadDto> ListarCaracteristicasDeCalidadPorMaterialSinExceptuadas(Guid instanceId, int materialId, int centroId);

        [OperationContract]
        IList<KmPorProveedorDto> ListarKmPorProveedorYCentro(int clienteId, int centroId);

        [OperationContract]
        bool ExisteOrdenCargaFas(string orden);

        [OperationContract]
        Resultado ActualizarBajaCTGDefinitivaManual(Guid id);

        [OperationContract]
        UltimoEstadoDto ObtenerEstadoUltimoRecorrido(Guid id);

        [OperationContract]
        GraficoCamionesHoraDto ObtenerGraficoDeCamionesPorHora(GraficoCamionesHoraDto model, int centroId);

        [OperationContract]
        GraficoToneladasRangoDeDiasDto ObtenerGraficoToneladasPorRangoDeDias(GraficoToneladasRangoDeDiasDto model, int centroId);

        [OperationContract]
        GraficoCamionesDiaDto ObtenerGraficoDeCamionesPorDia(GraficoCamionesDiaDto model, int centroId);

        [OperationContract]
        GraficoEficienciaHidraulicasDto ObtenerGraficoDeEficienciaHidraulicas(GraficoEficienciaHidraulicasDto model, int centroId, string codigoHidraulicaVagon);

        [OperationContract]
        ListaPaginada<PesoMaximoPorTipoVehiculoDto> ListarPaginadoPesoMaximoPorTipoVehiculo(Paginacion paginacion, int centroId);

        [OperationContract]
        PesoMaximoPorTipoVehiculoDto ObtenerPesoMaximoPorTipoVehiculo(int id);

        [OperationContract]
        IList<PesoMaximoPorTipoVehiculoDto> ListarPesoMaximoPorTipoVehiculoPorCentro(int centroId);

        [OperationContract]
        int? ObtenerPesoNetoMaximo(TipoVehiculo tipoVehiculo, int centroId);

        [OperationContract]
        StockDeEstablecimientoDto ObtenerStockDeEstablecimiento(int id);

        [OperationContract]
        ListaPaginada<StockDeEstablecimientoDto> ListarPaginadoStockDeEstablecimientos(string filtro, Paginacion paginacion);

        [OperationContract]
        decimal StockEPAutilizado(string codigoEstablecimiento, string cosecha);

        [OperationContract]
        Resultado ValidarStockEstablecimiento(int establecimientoId, Guid instanceId);

        [OperationContract]
        MailAvisoStockDto ObtenerDatosMailAvisoStock(Guid workflowId);

        [OperationContract]
        bool DescuentaPesoDescontado(string cosecha);

        [OperationContract]
        bool EsRecorridoSustentable(Guid instanceId);

        [OperationContract]
        bool ValidaStockEPA(Guid instanceId);

        [OperationContract]
        ListaPaginada<CosechaDto> ListarPaginadoCosechas(string filtro, Paginacion paginacion);

        [OperationContract]
        CosechaDto ObtenerCosecha(int id);

        [OperationContract]
        CentroDto ObtenerCentroPorCodigoSap(string codigo);

        [OperationContract]
        MaterialIdYDescripcionDto ObtenerMaterialIdYDescripcionPorCodigoSap(string codigo);

        [OperationContract]
        decimal TotalKilosDescuentos(CaladoDto calado, AnalisisDeCalidadDto analisis, int pesoNeto);

        [OperationContract]
        FotosDto ListarFotosCamion(Guid instanciaWorkflow, string actividad);

        [OperationContract]
        FotosDto ListarFotosCamionPorCargaDeCupo(int id);

        [OperationContract]
        FotosDto ListarFotosCamionPorTarjeta(string tarjeta, string actividad);

        [OperationContract]
        FotosDto ListarFotosQuiebre(int id);

        [OperationContract]
        byte[] ObtenerUltimaFotoPorTarjeta(string tarjeta, int puestoDeTrabajoId);

        [OperationContract]
        byte[] ObtenerFotoPorQuiebreDeBarrera(string fileName, DateTime fecha);

        [OperationContract]
        FotosDto ListarFotosCamionPorRecorrido(int id, string actividad);

        [OperationContract]
        int ObtenerCantidadCamionesRechazados(int centroId);

        [OperationContract]
        List<EstadoMaterialDto> ListarEstadoPlanta(int centroId, bool mostrarIngresos, bool esGrano);

        [OperationContract]
        List<CupoMobileDto> ListarEstadoCupos(int centroId);

        [OperationContract]
        ListaPaginada<ExcepcionEnvioCamaraDto> ListarPaginadoExcepcionEnvioCamara(string filtro, Paginacion paginacion);

        [OperationContract]
        bool MaterialEnviaASapAlmacenPredeterminado(Guid guid);

        [OperationContract]
        ValidarCupoDto ValidarCupo(string cupo, int centroId, string numeroCartaPorte);

        [OperationContract]
        string[] ObtenerCodigoDeCentroPorId(int centroId);

        [OperationContract]
        int ObtenerNumeroDeTicketImportacionGenerado();

        [OperationContract]
        void RecuperarFotosTemporalesPorTarjeta(Guid instanceId);

        [OperationContract]
        IList<MotivoHumedadManualDto> ListarMotivosHumedad();

        [OperationContract]
        ListaPaginada<RangosDeRedondeoDto> ListarPaginadoRangosDeRedondeo(string filtro, Paginacion paginacion);

        [OperationContract]
        IList<RangosDeRedondeoDto> ListarRangosDeRedondeoPorMaterial(int material);

        [OperationContract]
        RangosDeRedondeoDto ObtenerRangosDeRedondeo(int id);

        [OperationContract]
        BalanzaDto ObtenerBalanzaPorPuestoDeTrabajo(int puestoDeTrabajo, TipoVehiculo tipoVehiculo);

        [OperationContract]
        BalanzaDto ObtenerBalanzaAsociadaAPuestoAutomatico(int puestoDeTrabajo);

        [OperationContract]
        int ObtenerIdBalanzaAutomaticaPorPuestoDeTrabajo(int puestoDeTrabajoId);

        [OperationContract]
        RecorridoDto ObtenerRecorridoPesadaExportacion(Guid id);

        [OperationContract]
        List<string> ObtenerUsuariosQuiebreApertura();

        [OperationContract]
        List<string> ObtenerUsuariosQuiebreCierre();

        [OperationContract]
        List<string> ObtenerUsuariosReasignacionDeTarjeta();

        [OperationContract]
        List<string> ObtenerUsuariosContingencia();

        [OperationContract]
        List<string> ObtenerUsuarioEntregaHexano();

        [OperationContract]
        bool ValoresNoCorrespondenAEspecial(Guid workflowInstanceId, int materialId, int centroId);

        [OperationContract]
        CargaDeCupoDto ObtenerCupoPorRecorrido(int recorridoId);

        [OperationContract]
        CargaDeCupoDto ObtenerCupoPorId(int id);

        [OperationContract]
        BalanzaPuertoDto ObtenerBalanzaPuerto(int id);

        [OperationContract]
        IList<BalanzaPuertoDto> ListarBalanzasPuerto();

        [OperationContract]
        IEnumerable<int> ListarBalanzadasFaltantesPorRango(int id, int idFin, string numeroBalanza);

        [OperationContract]
        CargaDto ObtenerCarga(int id, string numeroBalanza);

        [OperationContract]
        ListaPaginada<CargaDto> ListarPaginadoCargas(CargaFiltroDto filtro, Paginacion paginacion);

        [OperationContract]
        IList<CargaDto> ListarCargasSinPaginado(CargaFiltroDto filtro);

        [OperationContract]
        ListaPaginada<CargaDto> ListarEmbarquePorBuques(CargaFiltroDto filtro, Paginacion paginacion);

        [OperationContract]
        ListaPaginada<BalanzadaDto> ListarPaginadoBalanzadas(int id, int? idFin, string numeroBalanza, bool? enviado, Paginacion paginacion);

        [OperationContract]
        ListaPaginada<VaporDto> ListarVapores(Paginacion paginacion, string filtro);

        [OperationContract]
        ListaPaginada<BodegaDto> ListarBodegas(Paginacion paginacion, string filtro);

        [OperationContract]
        IList<MaterialPuertoDto> ListaMaterialesPuerto();

        [OperationContract]
        IList<MaterialPuertoDto> ListaMaterialesPuertoConDescripcionCorta();

        [OperationContract]
        IList<AgenciaMaritimaPuertoDto> ListarAgenciasMaritimas();

        [OperationContract]
        IList<CoordinadorPuertoDto> ListarCoordinadores();

        [OperationContract]
        ListaPaginada<MaterialPuertoDto> ListarMaterialesPuerto(Paginacion paginacion, string filtro);

        [OperationContract]
        ListaPaginada<DestinoDto> ListarDestinos(Paginacion paginacion, string filtro);

        [OperationContract]
        IList<DestinoDto> ListarTodosDestinos();

        [OperationContract]
        IList<AgenciaControlPrivadoDto> ListarAgenciasControlPrivado();

        [OperationContract]
        IList<AgenteControlPrivadoDto> ListarAgentesControlPrivado();

        [OperationContract]
        IList<EstibaDto> ListarEstibas();

        [OperationContract]
        PlanoDeCargaDto ObtenerPlanoDeCarga(int planoDeCargaId);

        [OperationContract]
        ListaPaginada<ExportadorDto> ListarExportadores(Paginacion paginacion, string filtro);

        [OperationContract]
        IList<ExportadorDto> ListaExportadores();

        [OperationContract]
        BalanzadaDto ObtenerBalanzada(int id, string numeroBalanza);

        [OperationContract]
        IList<VideoCamaraDto> ListarVideoCamarasPuerto();

        ListaPaginada<ReportePesadaDto> ListarPaginadoCargasPorFecha(DateTime fechaInicio, DateTime FechaFin, Paginacion paginacion);

        [OperationContract]
        ListaPaginada<ReportePesadaDto> ListarConsultaDeCargasPorFecha(DateTime fechaInicio, DateTime fechaFin, string tipo, Paginacion paginacion);

        [OperationContract]
        ReportePesadaDto ListarPesadasOnline(string codigoBalanza);

        [OperationContract]
        ListaPaginada<ReportePesadaSeisHorasDto> ListarReporteDePesadasPorTurno(DateTime fechaInicio, DateTime fechaFin, Paginacion paginacion, int? ExportadorId, int? MaterialId);

        [OperationContract]
        ListaPaginada<CentroDto> ListarPaginadoCentrosConEstacionesMeteorologicas(Paginacion paginacion);

        [OperationContract]
        VaporDto ObtenerVapor(int id);

        [OperationContract]
        BodegaDto ObtenerBodega(int id);

        [OperationContract]
        ExportadorDto ObtenerExportador(int id);

        [OperationContract]
        DestinoDto ObtenerDestino(int id);

        [OperationContract]
        MaterialPuertoDto ObtenerMaterialPuerto(int id);

        [OperationContract]
        IList<VaporDto> BuscarVapores(string criteria);

        [OperationContract]
        VaporDto BuscarVapor(string criteria);

        [OperationContract]
        IList<BodegaDto> BuscarBodegas(string criteria);

        [OperationContract]
        BodegaDto BuscarBodega(string criteria);

        [OperationContract]
        IList<ExportadorDto> BuscarExportadores(string criteria);

        [OperationContract]
        ExportadorDto BuscarExportador(string criteria);

        [OperationContract]
        IList<DestinoDto> BuscarDestinos(string criteria);

        [OperationContract]
        DestinoDto BuscarDestino(string criteria);

        [OperationContract]
        IList<MaterialPuertoDto> BuscarMaterialesPuerto(string criteria);

        [OperationContract]
        MaterialPuertoDto BuscarMaterialPuerto(string criteria);

        [OperationContract]
        IList<AlmacenDto> BuscarAlmacenesPuerto(string criteria);

        [OperationContract]
        AlmacenDto BuscarAlmacenPuerto(string criteria);

        [OperationContract]
        ListaPaginada<BalanzaPuertoDto> ListarBalanzasPuertoPaginado(string filtro, Paginacion paginacion);

        [OperationContract]
        int TotalEmbarcado(int cargaInicial_id, string cargaInicial_numeroBalanza);

        [OperationContract]
        IList<BalanzadaDto> ObtenerBalanzadasParaEnviarASAP(int cargaInicial_Id, string cargaInicial_NumeroBalanza);

        [OperationContract]
        List<MotivoQuiebreBarreraDto> ObtenerUltimosMovimientosDispositivo(string codigoDispositivo);

        [OperationContract]
        OtroRecorridoDelChoferDto ObtenerOtroRecorridoDelChofer(int choferId);

        [OperationContract]
        bool CupoConsumido(string cupoParamatro, int centroId);

        [OperationContract]
        bool CupoTransmitido(string cupoParamatro, string nroCartaPorte);

        [OperationContract]
        NirsDto ObtenerNirs(int id);

        [OperationContract]
        IList<NirsDto> ListarNirs();

        [OperationContract]
        IList<NirsDto> ListarNirsPorCentro(int centroId);

        [OperationContract]
        ListaPaginada<NirsDto> ListarPaginadoNirs(string filtro, int centroId, Paginacion paginacion);

        [OperationContract]
        string ObtenerMaterialNirsCodigoProducto(int materialid);

        [OperationContract]
        IList<FotosDto> ListarHistoricoDeFotosPorPatente(int centroId, string actividad);

        [OperationContract]
        ReasignacionDeTarjetaDto ObtenerReasignacionDeTarjeta(int id);

        [OperationContract]
        double CaladosPorHora(DateTime desde, DateTime hasta);

        [OperationContract]
        RecorridoDto UltimoCalado(DateTime desde, DateTime hasta, int centroId);

        [OperationContract]
        IList<string> ObtenerTodasBalanzaPuertoAdministrativa();

        [OperationContract]
        int ObtenerProximoIdEmbarqueLiquido(string codigoBalanza);

        [OperationContract]
        IList<string> ListarBalanzasPuertoReales();

        [OperationContract]
        int obtenerIdBalanzadaSiguiente(int CargaInicial_Id, string NumeroBalanza);

        [OperationContract]
        ListaPaginada<ReportePesadaDto> ListarCargasHistoricas(DateTime desde, DateTime hasta, Paginacion paginacion);

        [OperationContract]
        ListaPaginada<ReportePesadaDto> ListarCargasOnline(DateTime desde, DateTime hasta, Paginacion paginacion);

        [OperationContract]
        ListaPaginada<ReciboMunicipalDto> ListarPaginadoReciboMunicipal(string filtro, Paginacion paginacion, int centroId);

        [OperationContract]
        IList<MaterialPorWorkflowDto> ListarMaterialesPorWorkflowCliente(int workflowId, int centroId, int clienteId);

        [OperationContract]
        UltimoEstadoDto ObtenerEstadoRecorrido(Guid id);

        [OperationContract]
        UltimoEstadoDto ObtenerMotivoAutorizarRecorrido(Guid id);

        [OperationContract]
        RegistroBalanzaPuertoDto ObtenerMayorRegistro(string numeroBalanza);

        [OperationContract]
        List<String> ListarEmailsPorRoles(List<string> rols);

        [OperationContract]
        IList<AdjuntoDto> ListarAdjuntosChofer(int inhabilitacionChoferId);

        [OperationContract]
        IList<AdjuntoDto> ListarAdjuntosCamion(int inhabilitacionCamionId);

        [OperationContract]
        string GuardarArchivoFotoDocumentoExterno(string archivo, string extension, string numeroDocumento);

        [OperationContract]
        void EliminarArchivoFotoDocumentoExterno(string ruta);

        [OperationContract]
        AdjuntoDto ObtenerAdjunto(int id);

        [OperationContract]
        DocumentoExternoDto ObtenerAdjuntoDocumentoExterno(int id);

        [OperationContract]
        ListaPaginada<ReglaDeAnalisisObligatorioDto> ListarReglaDeAnalisisObligatorioPaginado(Paginacion paginacion, string term, int centroId);

        [OperationContract]
        ReglaDeAnalisisObligatorioDto ObtenerReglaDeAnalisisObligatorio(int id);

        [OperationContract]
        IList<ReglaDeAnalisisObligatorioDto> ListarReglaDeAnalisisObligatorioActivas();

        [OperationContract]
        bool VerificarCorrespondeCaladoEnPlanta(Guid instanceId);

        [OperationContract]
        ListaPaginada<RecorridoDto> ListarPaginadoRecorridosPorModificarDocumentoDeIngreso(int centroId, Paginacion paginacion, ModificarDocumentoDeIngresoDto model);

        [OperationContract]
        IList<AlmacenDto> ListarAlmacenesPorMaterialYCentroSustentableMixto(int centroId, int materialId);

        [OperationContract]
        IList<PuestosDeCargaDescargaDto> ListarHidraulicasPorCriterioSustentable(int centroId, bool esSustentable, bool sustentableMixta, bool excluirEspeciales = false);

        [OperationContract]
        IList<MaterialDto> ListarMaterialesFiltroF515(int centroId);

        [OperationContract]
        bool ValidarCupoCartaPorte(string cupo, int centroId, string numeroCartaPorte);

        [OperationContract]
        List<string> ObtenerUsuariosAutorizarTiempoEnTransito();

        [OperationContract]
        ListaPaginada<ClienteDto> ListarClientes(string filtro, Paginacion paginacion);

        [OperationContract]
        FotosDto ObtenerFotoCPDeCartaDePortePorrecorrido(Guid id);

        [OperationContract]
        FotosDto ObtenerFotoCPDeCartasDePortePorrecorrido(List<int> id);

        [OperationContract]
        CartaPorteDto ObtenerCartaDePortePorrecorrido(int id);

        [OperationContract]
        List<string> ObtenerUsuariosAutorizarTiempoEnTransitoConfirmado();

        [OperationContract]
        List<string> ObtenerUsuariosAutorizarTiempoEnTransitoRechazado();

        [OperationContract]
        List<HistoricoInhabilitacionChoferDto> ListarHistoricoInhabilitacionChofer(int id);

        [OperationContract]
        List<HistoricoInhabilitacionCamionDto> ListarHistoricoInhabilitacionCamion(int id);

        [OperationContract]
        List<AutorizacionCamionDto> ListarAutorizacionCamionPorInhabilitacion(int id);

        [OperationContract]
        List<AutorizacionChoferDto> ListarAutorizacionChoferPorInhabilitacion(int id);

        [OperationContract]
        string ObtenerTarjetaPorCargaDeCupo(Guid instanceid);

        [OperationContract]
        IList<InstanciaWorkflowDto> ListarDatosDeWorkflowsPendientes(int centroId, int cantidad);

        [OperationContract]
        FotosDto ObtenerFotoPorPath(string path);

        [OperationContract]
        bool TomaFotoEnMesa(int centroId);

        [OperationContract]
        ImpresionReciboMunicipalRecorridoDto ObtenerRecorridoImpresionReciboMunicipalPorTarjeta(string numTarjeta);

        [OperationContract]
        PagoConMercadoPagoDto ObtenerPagoConMercadoPagoPorRecorridoId(int RecorridoId);

        [OperationContract]
        string ObtenerNumGaritaEntrada(int id);

        [OperationContract]
        FotosDto ObtenerFotosCartaPortePorNumero(string numero);

        [OperationContract]
        DocumentoExternoDto ObtenerDocumentoExterno(int id);

        [OperationContract]
        IList<Guid> ObtenerGuidVagones(Guid instanceId);

        [OperationContract]
        decimal PorcentajeMuestraAuditoria(Guid instanceId);

        [OperationContract]
        ListaPaginada<LoteAuditoriaListaDto> ListarPaginadoAuditoriaLote(BuscarLoteAuditoriaDto filtro, Paginacion paginacion);

        [OperationContract]
        bool ExistenMuestraEnvioACamaraAuditoriaPendientes(int materialId, int centroId);

        [OperationContract]
        bool TieneAgenteDecompras(Guid instanceId);

        [OperationContract]
        List<EstadisticasCaladoDto> ObtenerEstadisticasCalado(DateTime fechaDesde, DateTime fechaHasta, int idCentro);

        [OperationContract]
        bool EsPuestoEnContingencia(int idPuesto, bool granos);

        [OperationContract]
        bool ImprimeTicketSalida(int idCentro);

        [OperationContract]
        List<StockDeEstablecimientoDto> ListarCampaniaPorCuit(string cuit, string cosecha);

        [OperationContract]
        IList<CalleDto> ObtenerCallesPorCentro(int centroId);

        [OperationContract]
        IList<CallePorRecorridoDto> ObtenerEstadoDeCalle();

        [OperationContract]
        CalleDto ObtenerSiguienteCalle(int materialId);

        [OperationContract]
        bool EsPuestoFullAutomatizado(int puestoId);

        [OperationContract]
        ChoferDto ObtenerChoferPorNumeroDocumento(string numeroDocumento);

        [OperationContract]
        string ObtenerMotivoDemoraRecorrido(int id);

        [OperationContract]
        EstadoPuertoDto ObtenerEstadoPuerto();

        [OperationContract]
        List<string> ObtenerUsuariosLineUp();

        [OperationContract]
        string ObtenerDispositivosMaestroApertura(int puestoId);

        [OperationContract]
        string ObtenerDispositivosMaestroCierre(int puestoId);

        [OperationContract]
        TipoComercialDto ObtenerTipoComercialPorCodigoSap(string codigoSap);

        [OperationContract]
        string ObtenerCalleInterna(Guid id);

        [OperationContract]
        int ObtenerEspacioDisponible(TipoCalle tipoCalle, int materialId, TipoCalidad calidad);

        [OperationContract]
        int? ObtenerAlmacenPorRecorrido(int id);

        [OperationContract]
        InfoPatenteDeCalleDto ObtenerInfoPatente(string patente, int calleId);

        [OperationContract]
        PinchazosPorCaladaDto UltimoCambioPinchazo(int centroId);

        [OperationContract]
        List<string> ObtenerUsuariosCambioPinchazosPorCalada();

        [OperationContract]
        IList<CalleDto> ListarTodasLasCalles(int centroId);

        [OperationContract]
        IList<CallePorRecorridoDto> ListarCallePorRecorridoPorCalleId(int calleId);

        [OperationContract]
        IList<PuestosDeCargaDescargaDto> ListarHidraulicasPorCentro(int centroId);

        [OperationContract]
        WorkFlowsFiltradosDto ConsultarEstadoWorkflow(WorkFlowsFiltradosDto resultado);

        [OperationContract]
        IList<PuestoDeTrabajoDto> ListarPuestosDeBalanzasAutomaticas();

        [OperationContract]
        IList<BajaCTGRetransmisionDto> ListarBajaCTGError(int centroId);

        [OperationContract]
        bool BalanzaEnCero(int balanzaId);

        [OperationContract]
        IList<VideoCamaraDto> ObtenerCamarasPorNombrePc(string nombrePc, int centroId);

        [OperationContract]
        bool EsPuestoPausado(int puestoId);

        [OperationContract]
        IList<ATAPuertoDto> ListarATAPuerto();

        [OperationContract]
        IList<TipoDeBuquePuertoDto> ListarTipoDeBuquePuerto();

        [OperationContract]
        IList<UbicacionDeBuquePuertoDto> ListarUbicacionDeBuquePuerto();

        [OperationContract]
        BalanzaDto ObtenerBalanzaPorPuestoDeTrabajoSinTipoVehiculo(int puestoDeTrabajoId);

        [OperationContract]
        List<string> ObtenerUsuariosPlanoDeCarga();

        [OperationContract]
        bool ExistePagoRealizado(string patente);

        [OperationContract]
        IList<MensajeCartelLedDto> ListarMensajesCartelLed(string codigo);

        [OperationContract]
        IList<SentidoManoDeEmbarqueDto> ListarSentidoManoDeEmbarques();

        [OperationContract]
        IList<CeldaManoDeEmbarqueDto> ListarCeldaManoDeEmbarques();

        [OperationContract]
        ModuloDeCargaDto ObtenerModuloDeCarga(int moduloDeCargaId);

        [OperationContract]
        IList<MotivosLimpiezaDto> ListarMotivosLimpieza();

        [OperationContract]
        ModuloDeCargaListadoDto ObtenerUltimaHabilitacionDeTanques();

        [OperationContract]
        IList<MotivosDeCorteDto> ListarMotivosDeCorte();

        [OperationContract]
        IList<MotivosFallasBalanzaDto> ListarMotivosFallasBalanza();

        [OperationContract]
        IList<TurnoPuertoDto> ListarTurnoPuerto();

        [OperationContract]
        double ObtenerCantidadCubitacionDeTanques(string tk, double altura);

        //[OperationContract]
        //int ObtenerLlenadoMilimetroPorTanque(int cm, int mm, string tanqueNum);

        [OperationContract]
        decimal ObtenerDensidadPorTemperaturaDeMaterial(int materialPuertoId, int grado);

        [OperationContract]
        int ObtenerLlenadoMinimetroPorTanque(string cm, string mm, string tanqueNum);

        [OperationContract]
        IList<BalanzadaDto> ListarBalanzadaBuque(int buque);

        [OperationContract]
        IList<CargaDto> ListarCargaBalanzaPuerto();

        [OperationContract]
        Resultado ActualizarEstadoBuque(int Embarque_Id, int Estado);

        [OperationContract]
        string obtenerDireccionesDeMail(string templateMail);

        [OperationContract]
        ListaPaginada<ImpEtiquetaPuertoDto> ListarEtiquetasPuerto(int usuarioId, Paginacion paginacion);

        [OperationContract]
        int? ObtenerPesoNetoExportacion(Guid id);

        [OperationContract]
        NotificacionAplicacionDto ObtenerNotificacionAplicacion(string usuario);

        [OperationContract]
        ReporteDetalleMovimientoDto ReporteDetalleDeMovimiento(int centroId, DateTime fecha);

        [OperationContract]
        List<MaterialPorCentroDto> ListarMaterialGranoPorCentro(int centroId, bool esGrano);

        [OperationContract]
        IList<MaterialDto> ObtenerMaterialNoGranoAsignableCalle();

        [OperationContract]
        IList<WorkflowDto> ListarWorkFlowsPendientesNoGrano(int centroId);

        [OperationContract]
        MaterialPorCentroDto ObtenerMaterialPorCodigoAfip(int centroId, int material);

        [OperationContract]
        ProveedorDto ObtenerProveedorPorId(int Id);

        [OperationContract]
        IList<PuestoDeTrabajoDto> ListarPuestosDeTrabajoPorCodigoLectorQR(string Codigo);

        [OperationContract]
        List<ConfiguracionAutomatizacionEtapasDto> ObtenerConfiguracionAutomatizacionEtapas();

        [OperationContract]
        BajaCTGDto ObtenerBajaCTG(int cartaPorteid);

        [OperationContract]
        CartaPorteElectronicaDto ObtenerCartaPorteElectronicaPorCTG(string ctg);

        [OperationContract]
        List<long> ObtenerCpesNoCacheadas(List<CartaPorteResumenDto> ctgs);

        [OperationContract]
        int ObtenerSequenciaNumeroCTGCartaPorteElectronica();

        [OperationContract]
        bool? ObtenerSiEsCPEporWf(Guid instanceId);

        [OperationContract]
        IList<RamalFerroviarioDto> ListarRamalFerroviario();

        [OperationContract]
        bool ValidarCPERedespacho(string numero, int centroId, string workflowCodigo, int tipoVehiculo, bool consultactg = false);

        [OperationContract]
        IList<MotivoInactividadDto> ListarMotivosInactividad();

        [OperationContract]
        RegistroInactividadDto ObtenerRegistroInactividad(int id);

        [OperationContract]
        RegistroInactividadDto ObtenerUltimoRegistroInactividadPorUsuario(string usuario);

        [OperationContract]
        ControlRecorridoDto ObtenerUltimoCaladoPorPuestoDeTrabajo(int id);

        [OperationContract]
        EntidadTipoDeActividadDto ObtenerEntidadActividadPorCodigos(string codigoEntidad, string codigoTipoActividad);

        [OperationContract]
        IList<EntidadTipoDeActividadDto> ListarActividadesPorEntidad(string codigoEntidad);

        [OperationContract]
        MonitorCPECacheadaResultadoDto ListarCPEsCacheadas(MonitorCPECacheadaFiltroDto filtro, Paginacion paginacion);

        [OperationContract]
        List<MaterialDto> ListarMaterialGranosConCodigoONCCA();

        [OperationContract]
        CartaPorteElectronicaDto ObtenerCartaPorteElectronica(int id);

        [OperationContract]
        Resultado ActualizarFechaEstadoCacheadoCPECentro(int id, string mensaje);

        [OperationContract]
        string ObtenerDispositivoBarreraEntrada(int puestoId);

        [OperationContract]
        string ObtenerPuestoDeLogLecturaDeTarjeta(string patente, int? analisisDeCalidadId);

        [OperationContract]
        ConfiguracionGeneralDto ObtenerConfiguracionGeneral(string pantalla, string nombre, int? centroId = null);

        [OperationContract]
        List<ConfiguracionGeneralDto> ListarConfiguracionesGenerales(string pantalla, int? centroId = null);

        [OperationContract]
        List<ConfiguracionGeneralDto> ListarConfiguracionesGeneralesPorNombres(string pantalla, List<string> nombres, int? centroId = null);

        [OperationContract]
        ListaPaginada<VisualizacionBarreraDto> ListarPaginadoVisualizacionBarrera(int centroId, Paginacion paginacion);

        [OperationContract]
        IList<SensorBarreraDto> ListarSensoresBarreras(int grupoId);

        [OperationContract]
        VisualizacionBarreraDto ObtenerVisualizacionBarrera(int id);

        [OperationContract]
        IList<VisualizacionBarreraDto> ObtenerGruposBarrerasPorUsuario(string usuario, int centroId);

        [OperationContract]
        int ObtenerCantidadBarrerasPorUsuario(string usuario, int centroId);

        [OperationContract]
        IList<SensorBarreraDto> ListarSensoresBarrerasActivos();

        [OperationContract]
        CargaDeCupoDto ObtenerCupoPorCupoSap(string cupo);

        [OperationContract]
        ConfigSensoresDto ObtenerConfiguracionSensores(int id);

        [OperationContract]
        ListaPaginada<ConfigSensoresDto> ListarPaginadoConfigSensores(string filtro, Paginacion paginacion, int centroId);

        [OperationContract]
        IList<ConfigSensoresDto> ListarConfiguracionSensores(int centroId);

        [OperationContract]
        IList<VisualizacionBarreraDto> ObtenerGruposBarrerasPorCentro(int centroId);

        [OperationContract]
        List<EficienciaCaladoValoresDto> ObtenerEficienciaCalado(DateTime desde, DateTime hasta);

        [OperationContract]
        List<EficienciaCaladoValoresDto> ListarCallesCalado(int centroId);

        [OperationContract]
        EficienciaCaladoValoresDto ObtenerEficienciaCalle(int id, int centroId);

        [OperationContract]
        int ObtenerCantidadPendientesPorCalar(int centroId);

        [OperationContract]
        FotoDto ObtenerFotoSustentable(int centroId, string numeroDocumento, string actividad);

        [OperationContract]
        IList<LecturaPuestoDeTrabajoDto> ObtenerLogLecturasPorTarjeta(string tarjeta);

        [OperationContract]
        IList<SensorBarreraDto> ListarSensoresBarrerasActivosPorNombreDePC(string nombrePc);

        [OperationContract]
        bool AvanzaCpe(int centroId);

        [OperationContract]
        FotoDto ObtenerFotoDescargada(int centroId, string numeroDocumento, string actividad, bool esSustentable);

        [OperationContract]
        bool ValidarAltaCTGRepetida(string ctg);

        [OperationContract]
        bool ValidarAltaCPERepetida(string cpe, int sucursal);

        [OperationContract]
        MensajeCartelLedDto ObtenerMensajeCartelLedPorCodigo(string codigo);

        [OperationContract]
        IList<MuestraDeInaseDto> ObtenerLotesMuestrasInase();

        [OperationContract]
        IList<ConfiguracionGeneralDto> ObtenerConfiguracionMailInase(int centroId);

        [OperationContract]
        List<MuestraDeInaseDto> ObtenerMuestrasInaseParaArchivo();

        [OperationContract]
        Resultado ActualizarDispositivoLog(string codigo, string nombre, string valor, bool limpiarLogs);

        [OperationContract]
        List<string> ObtenerGruposBarreraEnUso(List<string> codigos);

        [OperationContract]
        List<LogDispositivoDto> ObtenerLogDispositivos(List<string> codigos);

        [OperationContract]
        ListaPaginada<LoteInaseDto> ListarPaginadoLoteInase(FiltroLoteInaseDto filtro, Paginacion paginacion);

        [OperationContract]
        ListaPaginada<MuestraDeInaseDto> ListarMuestrasPorLoteInase(int loteId, Paginacion paginacion);

        [OperationContract]
        string ObtenerNumeroLoteInase(int loteId);

        [OperationContract]
        LoteInaseDto ObtenerLoteInaseParaImpresion(int loteId);

        [OperationContract]
        bool EsCupoReingresado(string cupo, string nroCartaPorte, int centroId);

        [OperationContract]
        CargaDeCupoDto ObtenerCupoReingresado(string cupo, string nroCartaPorte, int centroId);

        [OperationContract]
        CalleDto CalcularCalle(TipoCalle tipoCalle, TipoCalidad tipoCalidad, int materialId, int centroId);

        [OperationContract]
        List<SensorBarreraDto> ListarSensoresBarrerasHidraulicasActivos();

        [OperationContract]
        CaladoDto ObtenerCaladoPorId(int id);

        [OperationContract]
        CargaDeCupoDto ObtenerCargaDeCupoPorGuid(Guid instanceId);

        [OperationContract]
        List<LlamadoAutomaticoHidraulicaDto> ListarHidraulicasPorEstado(EstadoHidraulica estado);

        [OperationContract]
        ConfiguracionCalleHidraulicaDto ObtenerConfiguracionCalleHidraulicaPorSensorCamaraALPR(string codigoSensor);

        [OperationContract]
        List<LlamadoAutomaticoHidraulicaDto> ListarHidraulicasAutomatizadas();

        [OperationContract]
        PuestosDeCargaDescargaDto ObtenerHidraulicaPorSensorBajada(string codigoSensorBajada);

        [OperationContract]
        RecorridoDto ObtenerRecorridoActivoPorPatente(string patente);

        [OperationContract]
        ConfiguracionCalleHidraulicaDto ObtenerConfiguracionCalleHidraulicaPorSensorCirculacion(string codigoSensor);

        [OperationContract]
        PuestosDeCargaDescargaDto ObtenerPuestoDeCargaDescargaPorPuestoId(int puestoDeTrabajoId);

        [OperationContract]
        ListaPaginada<ConfiguracionCalleHidraulicaDto> ListarPaginadoCalleHidraulica(Paginacion paginacion);

        [OperationContract]
        ConfiguracionCalleHidraulicaDto ObtenerCalleHidraulica(int id);

        [OperationContract]
        bool ExisteConfirmacionCargaDescargaDeRecorrido(int recorridoId);

        [OperationContract]
        IList<CalleDto> ListarCallesPorTipo(TipoCalle tipo);

        [OperationContract]
        string ObtenerCodigoMensaje(int calle);

        [OperationContract]
        int ObtenerOrdenCircular(string codigo);

        [OperationContract]
        int ObtenerCantidadCamionesLlamadosEnCallePreBalanza(int callePlayaInternaId);

        [OperationContract]
        List<CalleDto> ListarCallesPreBalanzaPorCallePlayaInternaId(int callePlayaInternaId);

        [OperationContract]
        CantidadPrecaladoCircularHelper ContarCallesBloqueadas(int? caladorId);

        [OperationContract]
        ListaPaginada<ExcepcionAlControlProveedorDto> ListarExcepcionesAlControlProveedor(string filtro, Paginacion paginacion);

        [OperationContract]
        ExcepcionAlControlProveedorDto ObtenerExcepcionAlControlProveedor(int id);

        [OperationContract]
        bool BuscarExcepcionAlControlProveedor(int materialId, int proveedorId, int centroId, DateTime fecha, int? centroDestinoId, int? clienteDestinoId);

        [OperationContract]
        bool ValidacionAutomaticaCtgDG(int centroId);

        [OperationContract]
        List<DomicilioDto> ListarDomicilios();

        [OperationContract]
        CartaPorteDerivadoGranarioDto ObtenerCartaPorteDerivadoGranarioPorGuid(Guid instanceId);

        [OperationContract]
        IList<VideoCamaraDto> ListarVideoCamarasPuesto(int idPuesto);

        [OperationContract]
        DatosDerivadoGranarioDto ObtenerDatoDerivadoGranarioPorRecorridoTipoDocumento(int recorridoId, TipoDocumentoIngreso tipoDocumentoIngreso);

        [OperationContract]
        ClienteDto ObtenerClientePorCuit(string cuit);

        [OperationContract]
        CartaPorteDerivadoGranarioDto ObtenerCartaPorteDerivadoGranarioPorCTG(string nroCTG);

        [OperationContract]
        MaterialDto ObtenerMaterialDerivadoGranario(short codigoPadre, short codigoDreivadoGranario);

        [OperationContract]
        DomicilioDto ObtenerDomicilioDG(int plantaDG);

        [OperationContract]
        IList<AlmacenDto> ListarAlmacenesPorMaterialYCentroEPA(int centroId, int materialId);

        [OperationContract]
        bool EsRecorridoConEstablecimiento(Guid instanceId);

        [OperationContract]
        int? EsRecorridoSojaEPAConAlmacen(Guid instanceId);

        [OperationContract]
        bool EsRecorridoSojaEPA(Guid instanceId);

        [OperationContract]
        bool RecorridoRepetidoEnElDia(string patente);

        [OperationContract]
        List<ClienteDto> ListarClientesPorCuit(string cuit);

        [OperationContract]
        List<ProveedorDto> ListarProveedoresPorCuit(string cuit, TiposProveedor tipo);

        [OperationContract]
        bool CaladorLleno(int calleCaladoId, int limiteFilasPrecaladoLlamadas);

        [OperationContract]
        int CaladoresActivos();

        [OperationContract]
        int ObtenerCalleInicial(Guid instanciaWorkflow);

        [OperationContract]
        bool EstaDisponibleCalle(int calleId);

        [OperationContract]
        IList<PuntoDeCargaDto> ListarPuntoDeCarga();

        [OperationContract]
        bool ExisteCalleConEspacioParaAsignarSegunTipoCalleYMaterial(TipoCalle tipoCalle, Guid workflowInstance);

        [OperationContract]
        bool ExisteExcepcionAlControlProveedorParaCartaPorte(int materialId, int proveedorId, int centroId, DateTime fecha, int? centroDestinoId, int? proveedorDestinoId);

        [OperationContract]
        bool ExisteExcepcionAlControlParaCartaPorte(int materialId, int transportistaId, int centroId, DateTime fecha, int? centroDestinoId, int? proveedorDestinoId);

        [OperationContract]
        int ObtenerMaterialIdPorInstanceId(Guid instanceId);

        [OperationContract]
        ListaPaginada<ComercialDto> ListarPaginadoComerciales(string filtro, Paginacion paginacion);

        [OperationContract]
        ComercialDto ObtenerComercial(int id);

        [OperationContract]
        List<ProveedorDto> ListarProveedores();

        [OperationContract]
        ProveedorInfoDto ObtenerProveedorInfoPorId(int id);

        [OperationContract]
        IList<ProveedorDto> ObtenerCorredoresAsociadosPorEstablecimientoId(int id);

        [OperationContract]
        List<ComercialDto> ListarComerciales();

        [OperationContract]
        IList<TipoVariedadPorMaterialDto> ListarTipoVariedadPorMaterial(int materialId);

        [OperationContract]
        IList<TipoVariedadDto> ListarTipoVariedad();

        [OperationContract]
        List<int> ListarIdCallesNoEditables();

        [OperationContract]
        InfoCalleDto ObtenerInfoCalle(int idCalle);

        [OperationContract]
        TipoVariedadDto[] ObtenerVariedadPorTipoMaterial(int idMaterial);

        [OperationContract]
        int? ObtenerVariedadIdPorMaterial(int materialId, string titularCP = null, bool esEpa = false, bool esSustentable = false);

        [OperationContract]
        IList<AutomatismoGranoDto> ListarAutomatismoGrano();

        [OperationContract]
        bool ExisteCamionesEnCalle(int calleId);

        [OperationContract]
        AutomatismoGranoDto ObtenerAutomatismoGranos(int id);

        [OperationContract]
        AutomatismoGranoDto ObtenerAutomatismoGranoPorRecorridoGuid(Guid workflowInstanceId);
        
        [OperationContract]
        List<AutomatismoNoGranoDto> ListarAutomatismoNoGrano();

        [OperationContract]
        IList<CalleDto> ListarCallesAutomatismoActivoPorTipoCalle(TipoCalle tipo);

        [OperationContract]
        IList<HistorialMensajeCartelLedDto> ListarCamionesLlamados();

        [OperationContract]
        IList<CalleDto> ListarCallesAutomatismoGrano(TipoCalle tipoCalle, int idCalleActual);

        [OperationContract]
        PuntoDeCargaDto ObtenerPuntoDeCarga(int id);

        [OperationContract]
        AutomatismoNoGranoDto ObtenerAutomatismoNoGrano(int id);

        [OperationContract]
        IList<PuntoDeCargaDto> ListarPuntosDeCargaActivosAutomatismoNoGrano();

        [OperationContract]
        IList<AlmacenDto> ListarAlmacenesActivosAutomatismoNoGrano();
    
		[OperationContract]    
        AsignacionAutomatismoGranoEnRecorridoDto ObtenerAsignacionAutomatismoGranoEnRecorrido(Guid workflowInstanceId);

        [OperationContract]
        bool ValidarDisponibilidadAsignacionEnCallePreBalanza(int callePBId, int callePHId);

        [OperationContract]
        CargaDeCupoDto ObtenerCargaDeCupoPorCTG(string nroCTG);

        [OperationContract]
        AutomatismoNoGranoDto ObtenerAutomatismoNoGranoActivoPorMaterialId(int materialId);

        [OperationContract]
        int? ObtenerCallePlantaIdPorMaterialId(int materialId);

        [OperationContract]
        MensajeCartelLedDto ObtenerCartelDisponible(string codigo);

        [OperationContract]
        RecorridoDto ObtenerPrimerRecorridoDisponibleParaLlamadoAutomaticoNoGranos();

        [OperationContract]
        IList<RecorridoDto> ObtenerPrimerosRecorridosDisponibleParaLlamadoAutomaticoNoGranos();

        [OperationContract]
        int ObtenerDisponibilidadEnCallePlantaNoGranos(int callePlantaNoGranosId);

        [OperationContract]
        CallePorRecorridoDto ObtenerCallePorRecorridoPlayaExternaNoGranosPorRecorridoId(int recorridoId);

        [OperationContract]
        int ObtenerMaterialIdPorCalleId(int calleId);

        [OperationContract]
        bool ExisteAsigacionGranoEnRecorrido(int recorridoId);

        [OperationContract]
        PuestosDeCargaDescargaDto ObtenerPuestoDeCargaDescarga(int puestoId);

        [OperationContract]
        bool EstaDisponibleParaLlamadoAutomaticoHidraulica(int hidraulicaId);

        [OperationContract]
        IList<AutomatismoTipoLlamadoDto> ListarAutomatismoTipoLlamado();

        [OperationContract]
        IList<AutomatismoNoGranoDto> ObtenerAutomatismosNoGranoActivoPorPuntoDeCarga(int puntoDeCargaId);

        [OperationContract]
        IList<AutomatismoNoGranoDto> ObtenerAutomatismosNoGranoActivos();
        
        [OperationContract]
        bool ValidarEspacioDisponibleEnCallePreHidraulica(int callePlayaInternaId);

        [OperationContract]
        int ObtenerCantidadCamionesEnCallePreHidraulica(int callePlayaInternaId);

        [OperationContract]
        bool ExisteEspacioDisponibleParaLlamarEnCartel(string codigo);

        [OperationContract]
        bool ExisteLlamadoCallePreBalanzaPorTipoDeLlamado(int callePHId, string codigoTipoLlamado);

        [OperationContract]
        IList<CallePreBalanzaPlayaInternaDto> ListarCallePreBalanzaLlamadasPorAutomatismo();

        [OperationContract]
        CallePorRecorridoDto ObtenerCallePorRecorrido(int calleId, int recorridoId);

        [OperationContract]
        CallePreBalanzaPlayaInternaDto ObtenerCallePreBalanzaPlayaInternaDeCamionEnEspera(int callePB);

        [OperationContract]
        CallePreBalanzaPlayaInternaDto ObtenerCallePrebalanzaPlayaInterna(int callePBId);

        [OperationContract]
        AsignacionNoGranoEnRecorridoDto ObtenerAsignacionNoGranoEnRecorridoPorRecorridoId(int recorridoId);

        [OperationContract]
        CallePorRecorridoDto ObtenerPrimerRecorridosDisponibleParaLlamadoAutomaticoGranos(List<int> callesPBIds);

        [OperationContract]
        CallePorRecorridoDto ObtenerCamionEnEsperaLlamadoGranos(int callePreBalanzaId);
        
        [OperationContract]
        IList<AutomatismoNoGranoDto> ObtenerAutomatismosNoGranoLlamadosActivos();

        [OperationContract]
        IList<AlmacenDto> ListarAlmacenesPorMateriaVariedadIds(List<int> tipoVariedadesIds, int materialId);
        
        [OperationContract]
        IList<RecorridoDto> ObtenerRecorridoNoRechazadoPorIdOperaciones(string numero);

    }

}