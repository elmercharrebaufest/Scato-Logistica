using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.ResultadoServicio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Molinos.Scato.Dominio;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearLogIdentificacionVehicular : ProcesadorComando<CrearLogIdentificacionVehicular>
    {
        private readonly IServicioComandos _servicioComandos;
        private readonly IServicioRepositorio _servicioRepositorio;
        private PuestoDeTrabajo _puesto;
        private DatosRecorridoDto _recorrido;

        public ProcesadorCrearLogIdentificacionVehicular(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioComandos servicioComandos, IServicioRepositorio servicioRepositorio)
            : base(repositorio, conversor, log)
        {
            _servicioComandos = servicioComandos;
            _servicioRepositorio = servicioRepositorio;
        }

        public override Resultado Ejecutar(CrearLogIdentificacionVehicular comando)
        {
            var resultado = new ResultadoProcesarIdentificacionVehicular();
            Log.Debug($"Procesando identificación vehicular: {comando.CodigoDispositivo} - {comando.Patente} - {comando.Tarjeta}");

            try
            {
                var logId = CrearLogAuditoria(comando);

                if (!Validar(comando, resultado, logId))
                {
                    ActualizarLogAuditoria(logId, resultado.Errores.Values.FirstOrDefault(), _puesto?.Id, _recorrido?.Id);
                    return resultado;
                }

                _servicioComandos.Ejecutar(new RegistrarMarcaDeTiempo
                {
                    Tipo = TipoRegistroMarcaDeTiempo.Identificacion,
                    PuestoDeTrabajoId = _puesto.Id,
                    NumeroDeTarjeta = comando.Tarjeta,
                    Patente = comando.Patente,
                    Trigger = comando.Trigger,
                });

                ActualizarLogAuditoria(logId, "Listo para avanzar workflow", _puesto.Id, _recorrido?.Id);

                resultado.LecturaPuestoDeTrabajo = CrearLecturaPuestoDeTrabajo(comando, logId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al procesar identificación vehicular");
                resultado.Error(nameof(Exception), ex.Message);
            }

            return resultado;
        }

        private int CrearLogAuditoria(CrearLogIdentificacionVehicular comando)
        {
            var log = new LogIdentificacionVehicular
            {
                CodigoDispositivo = comando.CodigoDispositivo,
                Tarjeta = comando.Tarjeta,
                ErrorDispositivo = comando.ErrorDispositivo,
                Patente = comando.Patente,
                PatenteLeida = comando.PatenteLeida,
                DiferenciaSustitucion = comando.DiferenciaSustitucion,
                VehiculoPresente = comando.VehiculoPresente,
                FechaEvento = comando.FechaEvento,
                ResultadoWorkflow = null,
                DuracionMecanismoSustitucionMs = comando.DuracionMecanismoSustitucionMs,
            };

            Repositorio.Agregar(log);

            if (comando.Detalles != null && comando.Detalles.Any())
            {
                foreach (var detalle in comando.Detalles)
                {
                    Repositorio.Agregar(new LogIdentificacionVehicularDetalle
                    {
                        LogIdentificacionVehicular = log,
                        ProveedorALPR = detalle.ProveedorALPR,
                        CodigoCamara = detalle.CodigoCamara,
                        RutaImagen = detalle.RutaImagen,
                        Intentos = detalle.Intentos,
                        Patente = detalle.Patente,
                        Certeza = detalle.Certeza,
                        Exitoso = (detalle.Patente != null && detalle.Patente.Equals(comando.Patente, StringComparison.InvariantCultureIgnoreCase)) || detalle.Exitoso,
                        Error = detalle.Error
                    });
                }
            }
            else
            {
                log.ErrorDispositivo = "No se obtuvo información de los dispositivos";
            }

            Repositorio.GuardarCambios();
            return log.Id;
        }

        private bool Validar(CrearLogIdentificacionVehicular comando, ResultadoProcesarIdentificacionVehicular resultado, int logId)
        {
            _puesto = Repositorio.ObtenerPrimero<PuestoDeTrabajo>(p => p.CodigoConfigIdentificacionVehicular == comando.CodigoDispositivo);
            if (_puesto == null)
            {
                resultado.Error(nameof(CrearLogIdentificacionVehicular), "Puesto no encontrado");
                return false;
            }

            _recorrido = _servicioRepositorio.ObtenerDatosRecorridoActivoPorTarjetaOPatente(comando.Trigger, comando.Patente, comando.Tarjeta);
            if (comando.Trigger == TipoIdentificacionPorPuesto.IngresoPorPatente && _recorrido == null && _puesto.AplicaContingenciaIdentificacionVehicularPorPanelAvanceManual)
            {
                resultado.Error(nameof(CrearLogIdentificacionVehicular), Constantes.ResultadoProcesoIdentificacionVehicular.EnviadoAContingencia);

                _servicioComandos.Ejecutar(new CrearLogAvanceManualCamion
                    {
                        PuestoDeTrabajoId = _puesto.Id,
                        NombreUsuario = null,
                        MotivoFallo = Constantes.ResultadoProcesoIdentificacionVehicular.EnviadoAContingencia,
                        PatenteLeida = comando.Patente,
                        Tarjeta = comando.Tarjeta,
                        LogIdentificacionVehicularOrigenId = logId
                    });

                resultado.EnviadoAContingencia = true;
                resultado.PuestosDeTrabajoId = new System.Collections.Generic.List<int> { _puesto.Id };
                return false;
            }

            return true;
        }

        private void ActualizarLogAuditoria(int logId, string resultadoWorkflow, int? puestoDeTrabajoId, int? recorridoId)
        {
            _servicioComandos.Ejecutar(new ActualizarLogIdentificacionVehicularResultado
            {
                LogId = logId,
                ResultadoWorkflow = resultadoWorkflow,
                PuestoDeTrabajoId = puestoDeTrabajoId,
                RecorridoId = recorridoId,
            });
        }

        private LecturaPuestoDeTrabajoDto CrearLecturaPuestoDeTrabajo(CrearLogIdentificacionVehicular comando, int? logId)
        {
            var lecturaPuestoDeTrabajo = new LecturaPuestoDeTrabajoDto
            {
                PuestoDeTrabajoId = _puesto.Id,
                CentroId = _puesto.Centro.Id,
                NumeroDeTarjeta = comando.Tarjeta,
                PuestoDeTrabajoPidePantente = _puesto.PidePatente,
                PuestoDeTrabajoImprimeTarjetaDeAcceso = _puesto.ImprimeTarjetaDeAcceso,
                Entrada = _puesto.Entradas(),
                Salida = _puesto.CierresEntrada(),
                Automatizado = _puesto.AutomatizadoFull && !_puesto.PausaAutoFull,
                VideoCamaras = !_puesto.PidePatente || _puesto.FotoAlMarcarTarjeta ? Conversor.ConvertirList<VideoCamara, VideoCamaraDto>(_puesto.VideoCamaras.ToList()) : new List<VideoCamaraDto>(),
                CodigoDispositivo = comando.CodigoDispositivo,
                Firmware = _puesto.Firmware,
                TarjetaValida = true,
                Patente = _recorrido != null ? _recorrido.Patente : null,
                ReconocimientoExitoso = _recorrido != null && _recorrido.Patente == comando.Patente,
                PatenteLeida = comando.Patente,
                TipoIdentificacion = comando.Trigger,
                LogIdentificacionVehicularId = logId,
            };

            if (comando.Trigger == TipoIdentificacionPorPuesto.IngresoPorLectura)
            {
                ValidarTarjeta(lecturaPuestoDeTrabajo, _puesto);
                EncolamientoLecturaTarjeta(lecturaPuestoDeTrabajo, _puesto);
            }

            return lecturaPuestoDeTrabajo;
        }

        private void ValidarTarjeta(LecturaPuestoDeTrabajoDto lecturaPuestoDeTrabajo, PuestoDeTrabajo puesto)
        {
            var esTarjetaBloqueada = _servicioRepositorio.EsTarjetaBloqueada(lecturaPuestoDeTrabajo.NumeroDeTarjeta, lecturaPuestoDeTrabajo.CentroId);
            if (esTarjetaBloqueada)
            {
                lecturaPuestoDeTrabajo.TarjetaValida = false;
                lecturaPuestoDeTrabajo.MensajeError = string.Format(Textos.Error_TarjetaBloqueada, lecturaPuestoDeTrabajo.NumeroDeTarjeta, puesto.NombrePuesto, lecturaPuestoDeTrabajo.CodigoDispositivo);
                return;
            }

            var esTarjetaRangoValida = _servicioRepositorio.EsTarjetaEnRangoValido(lecturaPuestoDeTrabajo.NumeroDeTarjeta, lecturaPuestoDeTrabajo.CentroId);
            if (!esTarjetaRangoValida)
            {
                lecturaPuestoDeTrabajo.TarjetaValida = false;
                lecturaPuestoDeTrabajo.MensajeError = string.Format(Textos.Error_TarjetaFueraDeRango, lecturaPuestoDeTrabajo.NumeroDeTarjeta, puesto.NombrePuesto, lecturaPuestoDeTrabajo.CodigoDispositivo);
                return;
            }

            var esTarjetaSupervisor = _servicioRepositorio.EsTarjetaSupervisor(lecturaPuestoDeTrabajo.NumeroDeTarjeta, lecturaPuestoDeTrabajo.CentroId, lecturaPuestoDeTrabajo.PuestoDeTrabajoId);
            if (esTarjetaSupervisor)
            {
                lecturaPuestoDeTrabajo.TarjetaValida = false;
                lecturaPuestoDeTrabajo.EsTarjetaSupervisor = true;
                lecturaPuestoDeTrabajo.DispositivosSupervisor = puesto.EntradasSupervisor();
                lecturaPuestoDeTrabajo.MensajeError = string.Format(Textos.Error_TarjetaSupervisor, lecturaPuestoDeTrabajo.NumeroDeTarjeta, puesto.NombrePuesto, lecturaPuestoDeTrabajo.CodigoDispositivo);
                return;
            }

            var esTarjetaEnUso = puesto.ImprimeTarjetaDeAcceso && !string.IsNullOrEmpty(lecturaPuestoDeTrabajo.Patente);
            if (esTarjetaEnUso)
            {
                lecturaPuestoDeTrabajo.TarjetaValida = false;
                lecturaPuestoDeTrabajo.MensajeError = string.Format(Textos.Error_TarjetaEnUso, lecturaPuestoDeTrabajo.NumeroDeTarjeta, puesto.NombrePuesto, lecturaPuestoDeTrabajo.CodigoDispositivo);
                return;
            }
        }

        private void EncolamientoLecturaTarjeta(LecturaPuestoDeTrabajoDto lecturaPuestoDeTrabajo, PuestoDeTrabajo puesto)
        {
            if (!lecturaPuestoDeTrabajo.TarjetaValida)
                return;

            if (puesto.PidePatente && puesto.EncolaLecturas)
            {
                var lecturaAnterior = Repositorio.ObtenerMayor<LecturaDeTarjeta, int>(x => x.PuestoDeTrabajo.Id == puesto.Id, x => x.Id);
                if (lecturaAnterior == null || lecturaAnterior.Lectura != lecturaPuestoDeTrabajo.NumeroDeTarjeta)
                {
                    Repositorio.Agregar(new LecturaDeTarjeta
                    {
                        PuestoDeTrabajo = puesto,
                        Lectura = lecturaPuestoDeTrabajo.NumeroDeTarjeta,
                        Patente = lecturaPuestoDeTrabajo.Patente
                    });
                    Repositorio.GuardarCambios();
                }
            }
            else
            {
                var lectura = puesto.Lecturas.LastOrDefault();
                if (lectura == null)
                {
                    lectura = new LecturaDeTarjeta
                    {
                        PuestoDeTrabajo = puesto
                    };
                    Repositorio.Agregar(lectura);
                }
                lectura.Lectura = lecturaPuestoDeTrabajo.NumeroDeTarjeta;
                lectura.Patente = lecturaPuestoDeTrabajo.Patente;
                Repositorio.GuardarCambios();
            }
            
            var primeraLectura = Repositorio.ObtenerMenor<LecturaDeTarjeta, int>(x => x.PuestoDeTrabajo.Id == puesto.Id, x => x.Id);
            lecturaPuestoDeTrabajo.PrimerNumeroDeTarjeta = primeraLectura == null ? "" : primeraLectura.Lectura;
        }
    }
}
