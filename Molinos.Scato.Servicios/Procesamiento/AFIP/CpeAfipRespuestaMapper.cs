using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios.AfipCPDigitalService;
using Molinos.Scato.Servicios.Helpers;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    /// <summary>
    /// Mapeo puro de la respuesta de AFIP/ARCA: validación de errores y conversión a
    /// <see cref="CartaPorteElectronica"/>. No accede a <see cref="Molinos.Scato.Repositorio.IRepositorio"/>
    /// ni persiste nada (ver <see cref="CartaPorteElectronicaCacheHelper"/> para eso). Compartido
    /// entre <see cref="ProcesadorConsultarCPDigital"/> (flujo interactivo, con enriquecimiento completo
    /// de CartaPorteDto) y <see cref="ProcesadorCachearCPEAfip"/> (job batch liviano), para evitar
    /// duplicar la lógica de mapeo/errores AFIP entre ambos procesadores.
    /// </summary>
    public static class CpeAfipRespuestaMapper
    {
        private static readonly List<string> ErroresNoBloqueantes = new List<string>
        {
            Constantes.AFIPCodigoDeError.ErrorPDFNoGenerado
        };

        /// <summary>
        /// Analiza los errores devueltos por AFIP y los agrega al resultado si corresponde.
        /// </summary>
        /// <returns>true si hay al menos un error bloqueante (no se debe continuar procesando la respuesta)</returns>
        public static bool TieneErrorBloqueante(dynamic respuesta, Resultado resultado, ILogger log)
        {
            if (respuesta == null)
            {
                resultado.Error(Constantes.AFIPCodigoDeError.NoExistenSolicitudes, "No se obtuvo respuesta desde AFIP");
                return true;
            }

            if (respuesta.errores == null || !((IEnumerable<dynamic>)respuesta.errores).Any())
                return false;

            var erroresNobloqueanteSet = new HashSet<string>(ErroresNoBloqueantes);
            var errores = (IEnumerable<dynamic>)respuesta.errores;
            var tieneErrorBloqueante = errores.Any(e => !erroresNobloqueanteSet.Contains((string)e.codigo));

            if (tieneErrorBloqueante)
            {
                var errorNoExiste = errores.Any(e => (string)e.codigo == Constantes.AFIPCodigoDeError.NoExistenSolicitudes);
                var mensaje = errorNoExiste ? "No se encuentran datos" : (string)errores.FirstOrDefault()?.descripcion;
                resultado.Error(Constantes.AFIPCodigoDeError.NoExistenSolicitudes, mensaje);
            }

            foreach (var error in errores)
                log.Error($"CpeAfipRespuestaMapper - ({error.codigo}) {error.descripcion}");

            return tieneErrorBloqueante;
        }

        /// <summary>
        /// Convierte la respuesta dinámica de AFIP (automotor o ferroviaria) en la entidad de caché
        /// CartaPorteElectronica. No accede a IRepositorio: es un mapeo puro.
        /// </summary>
        public static CartaPorteElectronica ConvertirResponseAFIPenCartaPorteElectronica(dynamic responseAFIP, ILogger log)
        {
            if (responseAFIP == null) return null;

            try
            {
                var cabeceraAFIP = responseAFIP.cabecera;
                var origenAFIP = responseAFIP.origen;
                var retiroProductorAFIP = responseAFIP.retiroProductor;
                var intervinientesAFIP = responseAFIP.intervinientes;
                var datosCargaAFIP = responseAFIP.datosCarga;
                var destinoAFIP = responseAFIP.destino;
                var destinatarioAFIP = responseAFIP.destinatario;
                var transporteAFIP = responseAFIP.transporte;

                // OrigenFerroviariaRespuesta (tren) no tiene nroRenspa: ese dato solo aplica al origen
                // de tipo unidad productiva en automotor. Se resuelve explícitamente según el tipo real
                // para evitar RuntimeBinderException al acceder vía dynamic.
                string nroRenspa = origenAFIP is OrigenFerroviariaRespuesta ? null : (origenAFIP != null ? origenAFIP.nroRenspa : null);

                // TransporteFerroviariaRespuesta (tren) no tiene dominio/cuitChofer/fechaHoraPartida/codigoTurno/tarifaReferencia:
                // usa vagón y conductor en su lugar. Acceder a esos campos vía dynamic sobre este tipo
                // lanza RuntimeBinderException, por eso se resuelven explícitamente según el tipo real.
                string dominio = string.Empty;
                long? cuitChofer = null;
                DateTime? fechaPartida = null;
                string codigoTurno = null;
                double tarifaReferencia = 0;
                int? codigoRamalAfip = null;
                string numeroPrecinto = null;
                long? nroOperativo = null;
                long? cuitTransportistaTramo2 = null;

                if (transporteAFIP is TransporteFerroviariaRespuesta transporteFerroviario)
                {
                    // El nro de vagón hace las veces de "dominio" para el vehículo ferroviario:
                    // ConvertirCartaPorteDto usa este campo (via CartaPorte.Dominio.Split(',')) para
                    // resolver la Patente del vagón. Sin esto, el vagón queda con Patente vacía.
                    dominio = transporteFerroviario.nroVagonSpecified
                        ? transporteFerroviario.nroVagon.ToString()
                        : string.Empty;
                    cuitChofer = transporteFerroviario.cuitConductor;
                    fechaPartida = transporteFerroviario.fechaHoraPartidaTrenSpecified
                        ? transporteFerroviario.fechaHoraPartidaTren
                        : (DateTime?)null;
                    // Ramal, precinto, nro de operativo y transportista de tramo 2 solo vienen en la
                    // respuesta ferroviaria; sin esto, el formulario nunca recibe estos datos al
                    // buscar CTG por tren.
                    codigoRamalAfip = transporteFerroviario.ramal?.codigo;
                    numeroPrecinto = transporteFerroviario.nroPrecinto != null && transporteFerroviario.nroPrecinto.Length > 0
                        ? string.Join(",", transporteFerroviario.nroPrecinto)
                        : null;
                    nroOperativo = transporteFerroviario.nroOperativoSpecified
                        ? transporteFerroviario.nroOperativo
                        : (long?)null;
                    cuitTransportistaTramo2 = transporteFerroviario.cuitTransportistaTramo2Specified
                        ? transporteFerroviario.cuitTransportistaTramo2
                        : (long?)null;
                }
                else if (transporteAFIP != null)
                {
                    if (transporteAFIP.dominio != null && transporteAFIP.dominio.Length > 0)
                        dominio = string.Join(",", transporteAFIP.dominio);
                    cuitChofer = transporteAFIP.cuitChofer;
                    fechaPartida = transporteAFIP.fechaHoraPartida;
                    codigoTurno = transporteAFIP.codigoTurno;
                    if (transporteAFIP.tarifaReferencia != null)
                        tarifaReferencia = Convert.ToDouble(transporteAFIP.tarifaReferencia);
                }

                var carta = new CartaPorteElectronica
                {
                    // cabecera
                    TipoCartaPorte = cabeceraAFIP != null ? cabeceraAFIP.tipoCartaPorte : (int?)null,
                    Sucursal = cabeceraAFIP != null ? cabeceraAFIP.sucursal : (int?)null,
                    NroOrden = cabeceraAFIP != null ? cabeceraAFIP.nroOrden : (long?)null,
                    NroCTG = cabeceraAFIP != null ? cabeceraAFIP.nroCTG : (long?)null,
                    FechaEmision = cabeceraAFIP != null ? cabeceraAFIP.fechaEmision : (DateTime?)null,
                    Estado = cabeceraAFIP != null ? cabeceraAFIP.estado : null,
                    FechaCP = cabeceraAFIP != null ? cabeceraAFIP.fechaInicioEstado : (DateTime?)null,
                    FechaVto = cabeceraAFIP != null ? cabeceraAFIP.fechaVencimiento : (DateTime?)null,
                    Observacion = cabeceraAFIP != null ? cabeceraAFIP.observaciones : null,

                    // origen
                    Provincia = origenAFIP != null ? origenAFIP.codProvincia : (int?)null,
                    Localidad = origenAFIP != null ? origenAFIP.codLocalidad : (int?)null,
                    Domicilio = origenAFIP != null ? origenAFIP.domicilio : null,
                    PlantaOrigen = origenAFIP != null ? origenAFIP.planta : (int?)null,
                    CuitOrigen = origenAFIP != null ? origenAFIP.cuit : (long?)null,
                    NroRenspa = nroRenspa,

                    // correspondeRetiroProductor
                    RetiroProductor = responseAFIP != null ? responseAFIP.correspondeRetiroProductor : (bool?)false,

                    // retiroProductor
                    CuitRemitenteComercialProductor = retiroProductorAFIP != null ? retiroProductorAFIP.cuitRemitenteComercialProductor : (long?)null,

                    // intervinientes
                    CuitRemitenteComercialVentaPrimaria = intervinientesAFIP != null ? intervinientesAFIP.cuitRemitenteComercialVentaPrimaria : (long?)null,
                    CuitRemitenteComercialVentaSecundaria = intervinientesAFIP != null ? intervinientesAFIP.cuitRemitenteComercialVentaSecundaria : (long?)null,
                    CuitIntermediario = retiroProductorAFIP != null ? retiroProductorAFIP.cuitRemitenteComercialProductor : (long?)0,
                    CuitMercadoATermino = intervinientesAFIP != null ? intervinientesAFIP.cuitMercadoATermino : (long?)null,
                    CuitCorredorVentaPrimaria = intervinientesAFIP != null ? intervinientesAFIP.cuitCorredorVentaPrimaria : (long?)null,
                    CuitCorredorVentaSecundaria = intervinientesAFIP != null ? intervinientesAFIP.cuitCorredorVentaSecundaria : (long?)null,
                    CuitRepresentanteEntregador = intervinientesAFIP != null ? intervinientesAFIP.cuitRepresentanteEntregador : (long?)null,
                    CuitRepresentanteRecibidor = intervinientesAFIP != null ? intervinientesAFIP.cuitRepresentanteRecibidor : (long?)null,
                    CuitRemitenteComercialVentaSecundaria2 = intervinientesAFIP != null ? intervinientesAFIP.cuitRemitenteComercialVentaSecundaria2 : (long?)null,

                    // datosCarga
                    Material = datosCargaAFIP != null ? datosCargaAFIP.codGrano : null,
                    PesoBruto = datosCargaAFIP != null ? datosCargaAFIP.pesoBruto : (double?)null,
                    PesoTara = datosCargaAFIP != null ? datosCargaAFIP.pesoTara : (double?)null,
                    Cosecha = datosCargaAFIP != null ? datosCargaAFIP.cosecha : (int?)null,

                    // destino
                    CuitDestino = destinoAFIP != null ? destinoAFIP.cuit : (long?)null,
                    LocalidadDestino = destinoAFIP != null ? destinoAFIP.codLocalidad : (int?)null,
                    ProvinciaDestino = destinoAFIP != null ? destinoAFIP.codProvincia : (int?)null,
                    PlantaDestino = destinoAFIP != null ? destinoAFIP.planta : (int?)null,

                    // destinatario
                    CuitDestinatario = destinatarioAFIP != null ? destinatarioAFIP.cuit : (long?)null,

                    // transporte
                    CuitTransportista = transporteAFIP != null ? transporteAFIP.cuitTransportista : (long?)null,
                    Dominio = dominio,
                    FechaPartida = fechaPartida,
                    KmRecorrer = transporteAFIP != null ? transporteAFIP.kmRecorrer : (int?)null,
                    CodigoTurno = codigoTurno,

                    CuitChofer = cuitChofer,
                    Tarifa = transporteAFIP != null && transporteAFIP.tarifa != null ? Convert.ToDouble(transporteAFIP.tarifa) : 0,
                    CuitPagadorFlete = transporteAFIP != null ? transporteAFIP.cuitPagadorFlete : (long?)null,
                    CuitIntermediarioFlete = transporteAFIP != null ? transporteAFIP.cuitIntermediarioFlete : (long?)null,
                    MercaderiaFumigada = transporteAFIP != null ? transporteAFIP.mercaderiaFumigada : (bool?)null,
                    FechaUltimaActualizacion = null,
                    Pdf = responseAFIP != null ? responseAFIP.pdf : null,
                    TarifaReferencia = tarifaReferencia,
                    FechaCacheado = DateTime.Now,
                    RamalFerroviario = codigoRamalAfip,
                    NumeroPrecinto = numeroPrecinto,
                    NroOperativo = nroOperativo,
                    CuitTransportistaTramo2 = cuitTransportistaTramo2
                };

                return carta;
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error en ConvertirResponseAFIPenCartaPorteElectronica");
                return null;
            }
        }
    }
}
