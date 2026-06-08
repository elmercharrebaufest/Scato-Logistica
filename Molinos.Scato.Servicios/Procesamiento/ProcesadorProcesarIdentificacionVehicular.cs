using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.ResultadoServicio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorProcesarIdentificacionVehicular : ProcesadorComando<ProcesarIdentificacionVehicular>
    {
        private PuestoDeTrabajo _puesto;
        private int? _recorridoId;

        public ProcesadorProcesarIdentificacionVehicular(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ProcesarIdentificacionVehicular comando)
        {
            var resultado = new ResultadoProcesarIdentificacionVehicular
            {
                AvanzarWorkflow = false
            };

            try
            {
                var logId = CrearLogAuditoria(comando);
                resultado.LogId = logId;

                Log.Debug($"IdentificacionVehicular — log creado. Id: {logId}, Tarjeta: {comando.Tarjeta}, CodigoDispositivo: {comando.CodigoDispositivo}");

                if (!Validar(comando, resultado, logId))
                    return resultado;

                resultado.AvanzarWorkflow = true;
                resultado.LecturaPuestoDeTrabajo = CrearLecturaPuestoDeTrabajo(_puesto, _recorridoId.Value, comando);

                Log.Debug($"IdentificacionVehicular — ready para avanzar workflow. RecorridoId: {_recorridoId}, PuestoId: {_puesto.Id}");
                ActualizarLogResultado(logId, "Listo para avanzar workflow", _recorridoId, _puesto.Id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al procesar identificación vehicular");
                resultado.Error(nameof(Exception), ex.Message);
            }

            return resultado;
        }

        private bool Validar(ProcesarIdentificacionVehicular comando, ResultadoProcesarIdentificacionVehicular resultado, int logId)
        {
            if (!string.IsNullOrEmpty(comando.Error))
            {
                Log.Warn($"IdentificacionVehicular — error reportado: {comando.Error}");
                ActualizarLogResultado(logId, $"Error: {comando.Error}", null, null);
                resultado.ResultadoWorkflow = $"Error: {comando.Error}";

                if (string.IsNullOrEmpty(comando.Patente))
                    return false;
            }

            _puesto = ObtenerPuestoPorConfigIdentificacionVehicular(comando.CodigoDispositivo);
            if (_puesto == null)
            {
                Log.Warn($"IdentificacionVehicular — puesto no encontrado para CodigoDispositivo: {comando.CodigoDispositivo}");
                ActualizarLogResultado(logId, "PuestoNoEncontrado", null, null);
                resultado.ResultadoWorkflow = "PuestoNoEncontrado";
                return false;
            }

            resultado.PuestoDeTrabajoId = _puesto.Id;

            _recorridoId = ObtenerIdRecorridoActivo(comando.Patente, comando.Tarjeta);
            if (_recorridoId == null)
            {
                Log.Warn($"IdentificacionVehicular — sin recorrido activo para tarjeta: {comando.Tarjeta} o patente: {comando.Patente} en el PuestoId: {_puesto.Id}");
                ActualizarLogResultado(logId, "SinRecorridoActivo", null, _puesto.Id);
                resultado.ResultadoWorkflow = "SinRecorridoActivo";
                return false;
            }

            resultado.RecorridoId = _recorridoId;
            return true;
        }

        private int CrearLogAuditoria(ProcesarIdentificacionVehicular comando)
        {
            var resultado = (ResultadoCrear)new ProcesadorCrearLogIdentificacionVehicular(Repositorio, Conversor, Log)
                .Ejecutar(new CrearLogIdentificacionVehicular
                {
                    CodigoDispositivo = comando.CodigoDispositivo,
                    Tarjeta = comando.Tarjeta,
                    Error = comando.Error,
                    Patente = comando.Patente,
                    VehiculoPresente = comando.VehiculoPresente,
                    FechaEvento = comando.FechaEvento == default ? DateTime.Now : comando.FechaEvento,
                    Detalles = comando.Detalles
                });
            return resultado.Id;
        }

        private void ActualizarLogResultado(int logId, string resultadoWorkflow, int? recorridoId, int? puestoDeTrabajoId)
        {
            new ProcesadorActualizarLogIdentificacionVehicularResultado(Repositorio, Conversor, Log)
                .Ejecutar(new ActualizarLogIdentificacionVehicularResultado
                {
                    LogId = logId,
                    ResultadoWorkflow = resultadoWorkflow,
                    RecorridoId = recorridoId,
                    PuestoDeTrabajoId = puestoDeTrabajoId
                });
        }

        private PuestoDeTrabajo ObtenerPuestoPorConfigIdentificacionVehicular(string codigoDispositivo)
        {
            return Repositorio.ObtenerPrimero<PuestoDeTrabajo>(p =>
                p.CodigoConfigIdentificacionVehicular == codigoDispositivo);
        }

        private LecturaPuestoDeTrabajoDto CrearLecturaPuestoDeTrabajo(PuestoDeTrabajo puesto, int recorridoId, ProcesarIdentificacionVehicular comando)
        {
            var recorrido = Repositorio.Obtener<Recorrido>(recorridoId);
            return new LecturaPuestoDeTrabajoDto
            {
                PuestoDeTrabajoId = puesto.Id,
                CentroId = puesto.Centro.Id,
                NumeroDeTarjeta = recorrido.TarjetaDeAcceso ?? string.Empty,
                PuestoDeTrabajoPidePantente = puesto.PidePatente,
                TarjetaValida = true,
                PuestoDeTrabajoImprimeTarjetaDeAcceso = puesto.ImprimeTarjetaDeAcceso,
                Entrada = puesto.Entradas(),
                Salida = puesto.CierresEntrada(),
                Automatizado = puesto.AutomatizadoFull && !puesto.PausaAutoFull,
                VideoCamaras = Conversor.ConvertirList<VideoCamara, VideoCamaraDto>(puesto.VideoCamaras.ToList()),
                Patente = comando.Patente,
                PatenteLeida = comando.Patente,
                CodigoDispositivo = comando.CodigoDispositivo,
                Firmware = puesto.Firmware,
                ReconocimientoExitoso = true,
                OcrActivo = true,
                PrimerNumeroDeTarjeta = recorrido.TarjetaDeAcceso ?? string.Empty,
                TipoIngresoPorPuesto = !string.IsNullOrEmpty(comando.Patente)
                                        ? TipoIngresoPorPuesto.IngresoPorPatente
                                        : !string.IsNullOrEmpty(comando.Tarjeta)
                                            ? TipoIngresoPorPuesto.IngresoPorLectura
                                            : TipoIngresoPorPuesto.IngresoPorPatente,
                                                };
        }

        private int? ObtenerIdRecorridoActivo(string patente, string tarjeta)
        {
            Recorrido recorrido = null;

            if (!string.IsNullOrEmpty(tarjeta))
            {
                recorrido = Repositorio.ObtenerPrimero<Recorrido>(r =>
                    !r.Terminado &&
                    r.TarjetaDeAcceso == tarjeta);

                Log.Debug(
                    "ObtenerIdRecorridoActivo por tarjeta {0}: {1}",
                    tarjeta,
                    recorrido?.Id.ToString() ?? "null");
            }

            if (recorrido == null && !string.IsNullOrEmpty(patente))
            {
                recorrido = Repositorio.ObtenerPrimero<Recorrido>(r =>
                    !r.Terminado &&
                    r.Patente == patente);

                Log.Debug(
                    "ObtenerIdRecorridoActivo por patente {0}: {1}",
                    patente,
                    recorrido?.Id.ToString() ?? "null");
            }

            return recorrido?.Id;
        }
    }
}
