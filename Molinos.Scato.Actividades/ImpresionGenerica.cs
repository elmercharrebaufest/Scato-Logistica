using System;
using System.Activities;
using System.Data.SqlTypes;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades
{
    public class ImpresionGenerica : CodeActivity<Resultado>
    {
        public InArgument<string> CTG { get; set; }
        public InArgument<DateTime?> FechaEmision { get; set; }
        public InArgument<DateTime?> FechaCP { get; set; }
        public InArgument<DateTime?> FechaVencimiento { get; set; }
        public InArgument<string> TitularCP { get; set; }
        public InArgument<string> CuitTitularCP { get; set; }
        public InArgument<string> Intermediario { get; set; }
        public InArgument<string> CuitIntermediario { get; set; }
        public InArgument<string> RtteComercial { get; set; }
        public InArgument<string> CuitRtteComercial { get; set; }
        public InArgument<string> Corredor { get; set; }
        public InArgument<string> CuitCorredor { get; set; }
        public InArgument<string> CorredorVendedor { get; set; }
        public InArgument<string> CuitCorredorVendedor { get; set; }
        public InArgument<string> IntermediarioFlete { get; set; }
        public InArgument<string> MercadoATermino { get; set; }
        public InArgument<string> CuitMercadoATermino { get; set; }
        public InArgument<string> CuitIntermediarioDelFlete { get; set; }
        public InArgument<string> Entregador { get; set; }
        public InArgument<string> CuitEntregador { get; set; }
        public InArgument<string> Destinatario { get; set; }
        public InArgument<string> CuitDestinatario { get; set; }
        public InArgument<string> Destino { get; set; }
        public InArgument<string> CuitDestino { get; set; }
        public InArgument<string> DireccionDestino { get; set; }
        public InArgument<string> LocalidadDestino { get; set; }
        public InArgument<string> ProvinciaDestino { get; set; }
        public InArgument<string> CodigoPostalDestino { get; set; }
        public InArgument<string> Transportista { get; set; }
        public InArgument<string> CuitTransportista { get; set; }
        public InArgument<string> Chofer { get; set; }
        public InArgument<string> CuitChofer { get; set; }
        public InArgument<string> Variedad { get; set; }
        public InArgument<string> Cosecha { get; set; }
        public InArgument<string> Procedencia { get; set; }
        public InArgument<string> CodigoEstablecimiento { get; set; }
        public InArgument<string> PesoBrutoOrigen { get; set; }
        public InArgument<string> PesoTaraOrigen { get; set; }
        public InArgument<string> PesoNetoOrigen { get; set; }
        public InArgument<string> Patente { get; set; }
        public InArgument<string> PatenteAcoplado { get; set; }
        public InArgument<string> KmARecorrer { get; set; }
        public InArgument<string> TarifaReferencia { get; set; }
        public InArgument<string> TarifaTonelada { get; set; }
        public InArgument<string> CodigoAnexo { get; set; }
        public InArgument<string> AcuerdoMarco { get; set; }
        public InArgument<string> Caratula { get; set; }
        public InArgument<string> PesoBruto { get; set; }
        public InArgument<string> PesoTara { get; set; }
        public InArgument<string> PesoNeto { get; set; }
        public InArgument<string> TipoDeComprobanteONCCA { get; set; }
        public InArgument<string> NumeroDeDocumentoDeIngreso { get; set; }
        public InArgument<string> SaldosSTOCK { get; set; }
        public InArgument<string> ObservacionesONCCA { get; set; }
        public InArgument<string> Observaciones { get; set; }
        public InArgument<bool> FletePagado { get; set; }

        public InArgument<string> NumeroCiu { get; set; }
        public InArgument<string> RazonSocialVinatero { get; set; }
        public InArgument<string> INVVinatero { get; set; }
        public InArgument<string> CuitVinatero { get; set; }
        public InArgument<string> IIBBVinatero { get; set; }
        public InArgument<int> TipoDeVehiculoId { get; set; }
        public InArgument<string> MarcaVehiculo { get; set; }
        public InArgument<string> ModeloVehiculo { get; set; }
        public InArgument<bool> EsUva { get; set; }
        public InArgument<bool> EsUvaPropia { get; set; }
        public InArgument<DateTime?> FechaPesoNetoBodega { get; set; }

        [RequiredArgument]
        public InArgument<string> CodigoDeImpresion { get; set; }

        [RequiredArgument]
        public InArgument<Guid> WorkflowId { get; set; }

        [RequiredArgument]
        public InArgument<int> PuestoDeTrabajoId { get; set; }

        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }

        [RequiredArgument]
        public InArgument<bool> EsDestinoCliente { get; set; }

        public InArgument<int> BocaDestinoId { get; set; }

        public InArgument<int> MaterialId { get; set; }

        public InArgument<int?> CantCopias { get; set; }

        public OutArgument<int?> ImpresionId { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var resultado = new ResultadoCrear();

            var workflowId = WorkflowId.Get<Guid>(context);
            var centroId = CentroId.Get<int>(context);
            var materialId = MaterialId.Get<int>(context);
            var codigo = CodigoDeImpresion.Get<string>(context);
            var bocaDestinoId = BocaDestinoId.Get<int>(context);
            var puestoDeTrabajoId = PuestoDeTrabajoId.Get<int>(context);

            var cTG = CTG.Get<string>(context);
            var fechaEmision = FechaEmision.Get<DateTime?>(context) ?? SqlDateTime.MinValue.Value;
            var fechaCP = FechaCP.Get<DateTime?>(context) ?? SqlDateTime.MinValue.Value;
            var fechaVencimiento = FechaVencimiento.Get<DateTime?>(context) ?? SqlDateTime.MinValue.Value;
            var titularCP = TitularCP.Get<string>(context);
            var cuitTitularCP = CuitTitularCP.Get<string>(context);
            var intermediario = Intermediario.Get<string>(context);
            var cuitIntermediario = CuitIntermediario.Get<string>(context);
            var rtteComercial = RtteComercial.Get<string>(context);
            var cuitRtteComercial = CuitRtteComercial.Get<string>(context);
            var corredor = Corredor.Get<string>(context);
            var cuitCorredor = CuitCorredor.Get<string>(context);
            var entregador = Entregador.Get<string>(context);
            var cuitEntregador = CuitEntregador.Get<string>(context);
            var destinatario = Destinatario.Get<string>(context);
            var cuitDestinatario = CuitDestinatario.Get<string>(context);
            var destino = Destino.Get<string>(context);
            var cuitDestino = CuitDestino.Get<string>(context);
            var direccionDestino = DireccionDestino.Get<string>(context);
            var localidadDestino = LocalidadDestino.Get<string>(context);
            var provinciaDestino = ProvinciaDestino.Get<string>(context);
            var codigoPostalDestino = CodigoPostalDestino.Get<string>(context);
            var esDestinoCliente = EsDestinoCliente.Get<bool>(context);
            var transportista = Transportista.Get<string>(context);
            var cuitTransportista = CuitTransportista.Get<string>(context);
            var chofer = Chofer.Get<string>(context);
            var cuitChofer = CuitChofer.Get<string>(context);
            var variedad = Variedad.Get<string>(context);
            var cosecha = Cosecha.Get<string>(context);
            var procedencia = Procedencia.Get<string>(context);
            var codigoEstablecimiento = CodigoEstablecimiento.Get<string>(context);
            var pesoBrutoOrigen = PesoBrutoOrigen.Get<string>(context);
            var pesoTaraOrigen = PesoTaraOrigen.Get<string>(context);
            var pesoNetoOrigen = PesoNetoOrigen.Get<string>(context);
            var patente = Patente.Get<string>(context);
            var patenteAcoplado = PatenteAcoplado.Get<string>(context);
            var kmARecorrer = KmARecorrer.Get<string>(context);
            var tarifaReferencia = TarifaReferencia.Get<string>(context);
            var tarifaTonelada = TarifaTonelada.Get<string>(context);
            var codigoAnexo = CodigoAnexo.Get<string>(context);
            var acuerdoMarco = AcuerdoMarco.Get<string>(context);
            var caratula = Caratula.Get<string>(context);
            var pesoBruto = PesoBruto.Get<string>(context);
            var pesoTara = PesoTara.Get<string>(context);
            var pesoNeto = PesoNeto.Get<string>(context);
            var tipoDeComprobanteONCCA = TipoDeComprobanteONCCA.Get<string>(context);
            var numeroDeDocumentoDeIngreso = NumeroDeDocumentoDeIngreso.Get<string>(context);
            var saldosSTOCK = SaldosSTOCK.Get<string>(context);
            var observacionesONCCA = ObservacionesONCCA.Get<string>(context);
            var observaciones = Observaciones.Get<string>(context);
            var fletePagado = FletePagado.Get<bool>(context);
            var cantCopias = CantCopias.Get<int?>(context) ?? 1;

            var corredorVendedor = CorredorVendedor.Get<string>(context);
            var cuitCorredorVendedor = CuitCorredorVendedor.Get<string>(context);
            var intermediarioFlete = IntermediarioFlete.Get<string>(context);
            var mercadoATermino = MercadoATermino.Get<string>(context);
            var cuitMercadoATermino = CuitMercadoATermino.Get<string>(context);
            var cuitIntermediarioDelFlete = CuitIntermediarioDelFlete.Get<string>(context);
            var numeroCiu = NumeroCiu.Get<string>(context);
            var razonSocialVinatero = RazonSocialVinatero.Get<string>(context);
            var iNVVinatero = INVVinatero.Get<string>(context);
            var cuitVinatero = CuitVinatero.Get<string>(context);
            var iIBBVinatero = IIBBVinatero.Get<string>(context);
            var fechaPesoNetoBodega = FechaPesoNetoBodega.Get<DateTime?>(context) ?? SqlDateTime.MinValue.Value;

            var marcaVehiculo = MarcaVehiculo.Get<string>(context);
            var modeloVehiculo = ModeloVehiculo.Get<string>(context);
            var esUva = EsUva.Get<bool>(context);
            var esUvaPropia = EsUvaPropia.Get<bool>(context);
            var tipoDeVehiculoId = TipoDeVehiculoId.Get<int>(context);

            var logActividad = new LogActividadDto
                {
                    Actividad = "Impresion Generica",
                    ActividadXaml = "ImpresionGenerica",
                    WorkflowInstanceId = workflowId,
                    Fecha = DateTime.Now
                };
            try
            {
                resultado = servicio.Ejecutar(new CrearLogActividad { Dto = logActividad }) as ResultadoCrear;
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.LogActividad_ErrorEnLaCarga);
            }

            try
            {
                var centroOrigen = repositorio.ObtenerCentro(centroId);
                var material = repositorio.ObtenerMaterial(materialId);
                var recorrido = repositorio.ObtenerRecorridoPorGuid(workflowId);
                var bocaDestino = repositorio.ObtenerBocaDestino(bocaDestinoId);
                var obs = repositorio.ObtenerObservacion(workflowId);
                if (obs != null && !String.IsNullOrEmpty(obs.Observaciones) && String.IsNullOrEmpty(observaciones))
                {
                    observaciones = obs.Observaciones;
                }

                string iNVVariedad = null;
                string tenorAzucarino = null;
                string iNVBodega = null;
                string cuitBodega = null;
                string iIBBBodega = null;
                string razonSocialBodega = null;
                if (esUva)
                {
                    var variedadEnt = repositorio.ObtenerVariedadPorMaterial(materialId);
                    variedad = variedadEnt != null ?  variedadEnt.Descripcion : null;
                    iNVVariedad = variedadEnt != null ? variedadEnt.NumeroINV : null;
                    tenorAzucarino = repositorio.ObtenerTenorAzucarino(workflowId);
                    iNVBodega = centroOrigen.NumeroINV;
                    cuitBodega = centroOrigen.Cuit;
                    iIBBBodega = centroOrigen.IngresosBrutos;
                    razonSocialBodega = centroOrigen.RazonSocial;
                }

                ProveedorDto prestador = null;
                if (bocaDestino != null)
                {
                    prestador = repositorio.ObtenerProveedor(bocaDestino.ProveedorId);
                }

                var documento = repositorio.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(codigo, centroId, puestoDeTrabajoId);
                if (documento == null || documento.FormatoDeImpresionId == null)
                {
                    throw new Exception("El Documento no posee formato de impresión");
                }
                var formato = repositorio.ObtenerFormatoDeImpresion(documento.FormatoDeImpresionId.Value);

                var dto = new ImpImpresionGenericaDto
                {
                    CTG = cTG,
                    FechaEmision = fechaEmision,
                    FechaCP = fechaCP,
                    FechaVencimiento = fechaVencimiento,
                    TitularCP = titularCP,
                    CuitTitularCP = cuitTitularCP,
                    Intermediario = intermediario,
                    CuitIntermediario = cuitIntermediario,
                    RtteComercial = rtteComercial,
                    CuitRtteComercial = cuitRtteComercial,
                    Corredor = corredor,
                    CuitCorredor = cuitCorredor,
                    Entregador = entregador,
                    CuitEntregador = cuitEntregador,
                    Destinatario = destinatario,
                    CuitDestinatario = cuitDestinatario,
                    Transportista = transportista,
                    CuitTransportista = cuitTransportista,
                    Chofer = chofer,
                    CuitChofer = cuitChofer,
                    Material = material != null? material.Descripcion : null,
                    Variedad = variedad,
                    Cosecha = cosecha,
                    Procedencia = procedencia,
                    CodigoEstablecimiento = codigoEstablecimiento,
                    PesoBrutoOrigen = pesoBrutoOrigen,
                    PesoNetoOrigen = pesoNetoOrigen,
                    PesoTaraOrigen = pesoTaraOrigen,
                    Patente = patente,
                    PatenteAcoplado = patenteAcoplado,
                    KmARecorrer = kmARecorrer,
                    TarifaReferencia = tarifaReferencia,
                    TarifaTonelada = tarifaTonelada,
                    CodigoAnexo = codigoAnexo,
                    Prestador = prestador != null ? prestador.Descripcion : "",
                    CuitPrestador = prestador != null ? prestador.Cuil : "",
                    DomicilioBocaDestino = bocaDestino != null ? bocaDestino.Domicilio : "",
                    ProvinciaBocaDestino = bocaDestino != null ? bocaDestino.Provincia : "",
                    LocalidadBocaDestino = bocaDestino != null ? bocaDestino.Localidad : "",
                    BocaDestino = bocaDestino != null ? bocaDestino.NombreBocaDeDestino : "",
                    AcuerdoMarco = acuerdoMarco,
                    Caratula = caratula,
                    PesoBruto = pesoBruto,
                    PesoNeto = pesoNeto,
                    PesoTara = pesoTara,
                    LocalidadCentroOrigen = centroOrigen.LocalidadDesc,
                    ProvinciaCentroOrigen = centroOrigen.ProvinciaDesc,
                    CodigoPostalCentroOrigen = centroOrigen.CodigoPostal,
                    DireccionCentroOrigen = centroOrigen.Direccion,
                    Destino = destino,
                    CuitDestino = cuitDestino,
                    LocalidadCentroDestino = !esDestinoCliente ? localidadDestino : string.Empty,
                    ProvinciaCentroDestino = !esDestinoCliente ? provinciaDestino : string.Empty,
                    CodigoPostalCentroDestino = !esDestinoCliente ? codigoPostalDestino : string.Empty,
                    DireccionCentroDestino = !esDestinoCliente ? direccionDestino : string.Empty,
                    LocalidadClienteDestino = esDestinoCliente ? localidadDestino : string.Empty,
                    ProvinciaClienteDestino = esDestinoCliente ? provinciaDestino : string.Empty,
                    DireccionClienteDestino = esDestinoCliente ? direccionDestino : string.Empty,
                    TipoDocumentoIngreso = recorrido.TipoDocumentoIngreso.ToString(),
                    TipoDeComprobanteONCCA = tipoDeComprobanteONCCA,
                    NumeroDeDocumentoDeIngreso = numeroDeDocumentoDeIngreso,
                    SaldosSTOCK = saldosSTOCK,
                    ObservacionesONCCA = observacionesONCCA,
                    Observaciones = observaciones,
                    WorkflowId = workflowId,
                    Codigo = codigo,
                    Impresora = documento.ImpresoraDireccion ?? "",
                    TipoDeWorkflow = recorrido.Workflow.TipoDeWorkflow,
                    FleteAPagar = fletePagado ? "" : "x",
                    FletePagado = fletePagado ? "x" : "",
                    EsSustentable = recorrido.EsSustentable,
                    NumeroCiu = numeroCiu,
                    INVBodega = iNVBodega,
                    CuitBodega = cuitBodega,
                    IIBBBodega = iIBBBodega,
                    RazonSocialBodega = razonSocialBodega,
                    FechaPesoNetoBodega = fechaPesoNetoBodega,
                    RazonSocialVinatero = razonSocialVinatero,
                    INVVinatero = iNVVinatero,
                    CuitViñatero = cuitVinatero,
                    IIBBViñatero = iIBBVinatero,
                    EsCamion = tipoDeVehiculoId == 1 ? "x" : "",
                    EsAcoplado = tipoDeVehiculoId == 2 ? "x" : "",
                    EsBines = tipoDeVehiculoId == 3 ? "x" : "",
                    EsMoliendaEnVinedos = tipoDeVehiculoId == 4 ? "x" : "",
                    EsTractor = tipoDeVehiculoId == 5 ? "x" : "",
                    MarcaVehiculo = marcaVehiculo,
                    ModeloVehiculo = modeloVehiculo,
                    INVVariedad = iNVVariedad,
                    TenorAzucarino = tenorAzucarino,
                    EsUvaPropia = esUvaPropia ? "x" : "",
                    EsUvaTerceros = !esUvaPropia ? "x" : "",
                    CuitCorredorVendedor = cuitCorredorVendedor,
                    CuitIntermediarioDelFlete = cuitIntermediarioDelFlete,
                    CuitMercadoATermino = cuitMercadoATermino,
                    CorredorVendedor = corredorVendedor,
                    IntermediarioFlete = intermediarioFlete,
                    MercadoATermino = mercadoATermino
                };

                resultado = servicio.Ejecutar(new ImprimirDocumentoDeImpresion { Dto = dto, FormatoDeImpresion = formato, CantidadCopias = cantCopias }) as ResultadoCrear;
                if (resultado != null)
                {
                    ImpresionId.Set(context, resultado.Id);
                }
            }
            catch (Exception e)
            {
                resultado.Errores.Add("1", e.Message);
            }

            try
            {
                resultado = servicio.Ejecutar(new FinDeActividad { InstanceId = workflowId, Actividad = "ImpresionGenerica", PuestoDeTrabajoId = puestoDeTrabajoId }) as ResultadoCrear;
            }
            catch (Exception)
            {
                resultado.Errores.Add("2", Textos.FinDeActividad_ErrorEnLaCarga);
            }

            return resultado;
        }
    }
}
