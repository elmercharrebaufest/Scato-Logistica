using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.ResultadoServicio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.GestionarCartasDePortePE;
using Molinos.Scato.Servicios.Impl;
using Molinos.Scato.Servicios.ServiciosSap;
using Ninject.Extensions.Logging;
using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorReprocesarPagoTasaMunicipal : ProcesadorComando<ReprocesarPagoTasaMunicipal>
    {
        private readonly IServicioComandos servicioComandos;

        public ProcesadorReprocesarPagoTasaMunicipal(IRepositorio repositorio, IServicioComandos servicioComandos, IConversor conversor, ILogger Log)
            : base(repositorio, conversor, Log)
        {
            this.servicioComandos = servicioComandos;
        }

        public override Resultado Ejecutar(ReprocesarPagoTasaMunicipal comando)
        {
            var resultado = new ResultadoReprocesarPagoTasaMunicipal();
            try
            {
                var contingencia = Repositorio.ObtenerMasReciente<Contingencia>(x => x.TipoContingencia == Constantes.Contingencia.PayCaido, x => x.Fecha);
                if(contingencia != null && contingencia.Activado)
                {
                    resultado.Error(string.Empty, "No se puede reprocesar el pago de la tasa municipal porque el sistema de pagos está en contingencia.");
                    return resultado;
                }

                Log.Debug("Obteniendo recorrido id: " + comando.Id);
                var recorrido = Repositorio.Obtener<Recorrido>(r => r.Id == comando.Id);
                if (recorrido == null)
                {
                    resultado.Error(string.Empty, "No se encontró el recorrido.");
                    return resultado;
                }

                var pagos = Repositorio.Listar<PagosTasaMunicipal>(p => p.IdInstance == recorrido.InstanciaWorkflow && p.NroRecibo != null );
                if (pagos != null && pagos.Count > 0)
                {
                    foreach (var item in pagos)
                    {
                        var informarPago = InformarPago(item.IdMOAPay);
                        if (informarPago.HayErrores)
                        {
                            resultado.Error(string.Empty, "Ocurrió un error al informar el pago de la tasa municipal.");
                            return resultado;
                        }
                    }
                    return resultado;
                }

                var resultadoVerificacion = servicioComandos.Ejecutar(new VerificarPagoTasaMunicipal
                {
                    CentroId = recorrido.Centro.Id,
                    Patente = recorrido.Patente,
                    Ctg = recorrido.NumeroDocumentoIngreso,
                    InstanceId = recorrido.InstanciaWorkflow,
                    MaterialId = recorrido.Material.Id,
                    PatenteAcoplado = recorrido.Vehiculo != null ? recorrido.Vehiculo.PatenteAcoplado : string.Empty,
                    TipoOrigenDeValidacion = TipoOrigenDeValidacion.Recorrido,
                    TipoVehiculo = recorrido.TipoVehiculo,
                    Usuario = comando.Usuario,
                });

                var resultadoPago = resultadoVerificacion as ResultadoConsultarPagoTasaMunicipal;
                if (resultadoPago != null && resultadoPago.HayErrores)
                {
                    string mensajeError = $"Error al verificar el pago de tasa municipal: {string.Join(", ", resultadoPago.Errores.Select(e => $"{e.Key}: {e.Value}"))}";
                    Log.Error(mensajeError);
                    throw new Exception(mensajeError);
                }

                if (resultadoPago != null && resultadoPago.EsPagoAbonado && resultadoPago.IdExcepcion == 0)
                {
                    var pagosVerificados = Repositorio.Listar<PagosTasaMunicipal>(x => x.IdInstance == recorrido.InstanciaWorkflow);
                    if (pagosVerificados != null && pagosVerificados.Count > 0)
                    {
                        var datosRecorrido = ObtenerImpresionReciboMunicipal(x => x.InstanciaWorkflow == recorrido.InstanciaWorkflow);
                        var codigoDocumentoImpresion = Constantes.ConfiguracionGeneral.ImpresionReciboMunicipal.Actividad;
                        Log.Info($"Se encontraron {pagosVerificados.Count} pagos digitales asociados al recorrido.");
                        foreach (var pagoVerificado in pagosVerificados)
                        {
                            Log.Debug($"Obteniendo número de ticket para la Patente: {recorrido.Patente}.");
                            var ticket = ObtenerNumeroDeTicketDigital(pagoVerificado.IdMOAPay, comando.PuestoDeTrabajoId);
                            Log.Info($"Número de puesto de trabajo: {comando.PuestoDeTrabajoId}, Número de ticket: {ticket}.");
                            var documento = ObtenerDocumentoDeImpresion(comando.PuestoDeTrabajoId, codigoDocumentoImpresion, recorrido.Centro.Id);
                            ImprimirReciboPagoDigital(documento, datosRecorrido, ticket, pagoVerificado.Importe.ToString(), pagoVerificado.Id, codigoDocumentoImpresion, recorrido.InstanciaWorkflow, 1);
                        }
                    }
                }
                else if (resultadoPago != null && !resultadoPago.EsPagoAbonado && resultadoPago.TieneDiferenciaDePago)
                {
                    resultado.Error(string.Empty, "No se pudo procesar, tiene diferencia de pago");
                }
                else
                {
                    resultado.Error(string.Empty, "No se pudo procesar, no existen pagos disponibles.");
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error al reprocesar pago tasa municipal", ex.Message);
                resultado.Error(string.Empty, "Ocurrió un error al reprocesar el pago de la tasa municipal.");
            }

            Repositorio.GuardarCambios();
            return resultado;
        }

        private Resultado InformarPago(int idPago)
        {
            Log.Info($"Informar con IdPago: {idPago}");
            var respuestaInformarPago = servicioComandos.Ejecutar(new MOAPayInformarPagoComoConsumido
            {
                Id = idPago,
                Disponible = "N"
            });

            if (respuestaInformarPago == null)
                throw new InvalidOperationException("No se pudo obtener el resultado al informar el pago.");

            if (respuestaInformarPago.HayErrores)
                throw new InvalidOperationException("No se pudo informar el pago de la tasa municipal.");

            return respuestaInformarPago;
        }

        private void ImprimirReciboPagoDigital(DocumentoDeImpresionPorCentro documento, ImpresionReciboMunicipalRecorridoDto recorrido, string ticket, string monto, int idPago, string codigo, Guid workflowId, int cantCopias)
        {
            var dto = new ImpReciboMunicipalDto
            {
                Impresora = documento?.Impresora?.Direccion ?? "",
                Codigo = codigo,
                TicketNro = ticket,
                Ordenanza = recorrido.Ordenanza,
                Valor = monto,
                FechaImpresion = DateTime.Now,
                WorkflowId = workflowId,
                Patente = recorrido.Patente,
                TipoVehiculo = recorrido.TipoVehiculo,
                NroDocumentoLegal = ObtenerDocumentoLegal(recorrido)
            };
            Log.Debug($"Imprimiendo {cantCopias} copia(s).");
            var resultado = servicioComandos.Ejecutar(new ImprimirReciboMunicipal { Dto = dto, CantidadCopias = cantCopias, IdPagoDigital = idPago });
            if (resultado.HayErrores)
                throw new Exception("Error al imprimir recibo: " + string.Join(", ", resultado.Errores.Select(e => $"{e.Key}: {e.Value}")));
        }

        private string ObtenerDocumentoLegal(ImpresionReciboMunicipalRecorridoDto recorrido)
        {
            if (recorrido.TipoDocumentoIngreso == TipoDocumentoIngreso.Remito)
            {
                return recorrido.DocumentoInternoSap ?? "";
            }
            if (recorrido.TipoDocumentoIngreso == TipoDocumentoIngreso.CartaPorte ||
                recorrido.TipoDocumentoIngreso == TipoDocumentoIngreso.OrdenDeDescargaFason)
            {
                return recorrido.NumeroDocumentoIngreso ?? "";
            }
            return "";
        }

        private string ObtenerNumeroDeTicketDigital(int idPago, int puestoDeTrabajoId)
        {
            var numGarita = ObtenerNumGaritaEntrada(puestoDeTrabajoId);
            if (numGarita == null)
                throw new Exception($"Garita no encontrada para puesto {puestoDeTrabajoId}");

            numGarita = numGarita.PadLeft(4, '0');
            var ticket = idPago.ToString().PadLeft(7, '0');
            return $"2{numGarita.Substring(1)}-{ticket}";
        }

        private string ObtenerNumGaritaEntrada(int id)
        {
            var nombrePuesto = Repositorio.ObtenerProyeccion<PuestoDeTrabajo, string>(x => x.Id == id, x => x.NombrePuesto);
            if (string.IsNullOrEmpty(nombrePuesto))
            {
                return "1";
            }
            var numeroPuesto = string.Concat(nombrePuesto.Where(Char.IsDigit));
            return !string.IsNullOrEmpty(numeroPuesto) ? numeroPuesto : "1";
        }

        private DocumentoDeImpresionPorCentro ObtenerDocumentoDeImpresion(int puestoDeTrabajoId, string codigo, int centroId)
        {
            DocumentoDeImpresionPorCentro documento = null;
            if (puestoDeTrabajoId != 0)
            {
                documento = Repositorio.Obtener<DocumentoDeImpresionPorCentro>(x => x.DocumentoDeImpresion.Codigo == codigo && x.Centro.Id == centroId && x.PuestoDeTrabajo.Id == puestoDeTrabajoId);
            }
            return documento ?? Repositorio.Obtener<DocumentoDeImpresionPorCentro>(x => x.DocumentoDeImpresion.Codigo == codigo && x.Centro.Id == centroId && x.PuestoDeTrabajo == null);
        }

        private ImpresionReciboMunicipalRecorridoDto ObtenerImpresionReciboMunicipal(Expression<Func<Recorrido, bool>> condicion)
        {
            var recorrido = Repositorio.ObtenerMayor<Recorrido, int>(condicion, x => x.Id);
            var hoy = DateTime.Now;
            var reciboMunicipal = Repositorio.ObtenerMayor<ReciboMunicipal, int>(x => x.Centro.Id == recorrido.Centro.Id && hoy >= x.FechaActivacion &&
                x.TipoVehiculo == recorrido.TipoVehiculo, x => x.Id);

            if (reciboMunicipal == null)
            {
                reciboMunicipal = Repositorio.ObtenerMayor<ReciboMunicipal, int>(x => x.Centro.Id == recorrido.Centro.Id && hoy >= x.FechaActivacion &&
                    x.TipoVehiculo == null, x => x.Id);
            }
            var pagoConMercadoPago = Repositorio.Existe<PagoConMercadoPago>(x => x.Recorrido.Id == recorrido.Id && !string.IsNullOrEmpty(x.MercadoPagoId) && !x.Devuelto);
            var pagoDigital = Repositorio.Existe<PagosTasaMunicipal>(p => p.IdInstance == recorrido.InstanciaWorkflow && p.Disponible == false);

            return new ImpresionReciboMunicipalRecorridoDto
            {
                DocumentoInternoSap = recorrido?.DocumentoInternoSap,
                NumeroDocumentoIngreso = recorrido?.NumeroDocumentoIngreso,
                Ordenanza = reciboMunicipal != null ? reciboMunicipal.Ordenanza : string.Empty,
                Monto = reciboMunicipal != null ? reciboMunicipal.Monto.ToString(CultureInfo.InvariantCulture) : string.Empty,
                TipoDocumentoIngreso = recorrido.TipoDocumentoIngreso,
                TipoVehiculo = recorrido.TipoVehiculo,
                Patente = recorrido.Patente,
                RecorridoId = recorrido.Id,
                NombreTransportista = recorrido.Transportista.RazonSocial,
                PagoConMercadoPago = pagoConMercadoPago,
                MedioDePago = pagoDigital ? MedioDePago.Digital : recorrido.Transportista.MedioDePago,
                Material = AutoMapper.Mapper.Map<Material, MaterialDto>(recorrido.Material),
                EstaPagadoPorMetodoDigital = pagoDigital
            };
        }
    }
}
