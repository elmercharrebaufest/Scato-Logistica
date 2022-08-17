using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios.Behavior;
using Ninject.Extensions.Logging;
using System;
using System.Linq;

namespace Molinos.Scato.Servicios.Impl
{
    [AiErrorHandlerBehaviorAttribute]
    public class ServicioSuscriptor : IServicioSuscriptor
    {
        private readonly IServicioComandos servicioComandos;
        private readonly ILogger log;
        private readonly IServicioRepositorio repositorio;
        private readonly IServicioEstadoPuesto estadoPuesto;

        public ServicioSuscriptor(IServicioComandos servicioComandos, ILogger log, IServicioRepositorio repositorio, IServicioEstadoPuesto estadoPuesto)
        {
            this.servicioComandos = servicioComandos;
            this.log = log;
            this.repositorio = repositorio;
            this.estadoPuesto = estadoPuesto;
        }

        public void Recibir(NotificacionEvento notificacion)
        {
            log.Debug("Notificacion recibida: {0}", notificacion);
            try
            {
                switch (notificacion.CodigoEvento)
                {
                    case "EntradaActivada":
                        var resultadoApertura = servicioComandos.Ejecutar(new CrearMotivoQuiebreBarrera { CodigoDispositivo = notificacion.CodigoDispositivo, Apertura = true });
                        EnviarMail(Textos.MailQuiebreBarrera, resultadoApertura, notificacion);
                        break;

                    case "EntradaDesactivada":
                        var resultadoCierre = servicioComandos.Ejecutar(new CrearMotivoQuiebreBarrera { CodigoDispositivo = notificacion.CodigoDispositivo, Apertura = false });
                        EnviarMail(Textos.MailCierreBarrera, resultadoCierre, notificacion);
                        break;

                    case "BalanzadaRecibida":
                        if (notificacion.Datos["tipoBalanzada"] == "fin")
                        {
                            servicioComandos.Ejecutar(new ValidarConsistenciaBalanzadas { Balanza = notificacion.CodigoDispositivo, CodigoDispositivo = notificacion.CodigoDispositivo, Hasta = Int32.Parse(notificacion.Datos["id"]) });
                        }
                        break;

                    case "CambioEstadoSensor":
                        estadoPuesto.NotificarSensorBarrera(notificacion);
                        bool estado;
                        if (bool.TryParse(notificacion.Datos["Mensaje"], out estado))
                        {
                            repositorio.ActualizarDispositivoLog(notificacion.CodigoDispositivo, "EstadoSensor", estado.ToString());
                            estadoPuesto.NotificarCambioDeEstado(notificacion.CodigoDispositivo, estado);
                        }
                        //else
                        //{
                        //    estadoPuesto.NotificarCambioDeEstado(notificacion.CodigoDispositivo, notificacion.Datos["Mensaje"]);
                        //}
                        break;

                    case "LecturaCPE":
                        LecturaCartaPorteElectronica(notificacion.CodigoDispositivo, int.Parse(notificacion.Datos["QR"]));
                        break;
                }
            }
            catch (Exception e)
            {
                log.Error(e, "No se pudo procesar la notificacion");
                throw;
            }
            log.Debug($"Notificacion {notificacion.CodigoEvento} procesada con exito");
        }

        private void EnviarMail(string cuerpo, Resultado resultadoApertura, NotificacionEvento notificacion)
        {
            if (!resultadoApertura.HayErrores)
            {
                var ultimosMovimientos = repositorio.ObtenerUltimosMovimientosDispositivo(notificacion.CodigoDispositivo);
                var ultMovString = "";

                foreach (MotivoQuiebreBarreraDto motivo in ultimosMovimientos)
                {
                    ultMovString += (motivo.Apertura == true ? "Quiebre " : "Cierre ") + motivo.Fecha + motivo.Hora + "<br/>";
                }

                var usuariosApertura = repositorio.ObtenerUsuariosQuiebreApertura();
                var resultado = resultadoApertura as ResultadoMotivoQuiebre;
                byte[] foto = null;
                if (resultado.Fotos.Any())
                {
                    foto = repositorio.ObtenerFotoPorQuiebreDeBarrera(resultado.Fotos.First(), DateTime.Now);
                }
                servicioComandos.Ejecutar(new EnvioMail { Destinatarios = usuariosApertura, Titulo = "Quiebre de Barrera", Cuerpo = string.Format(cuerpo, notificacion.CodigoDispositivo, DateTime.Now, notificacion.CodigoDispositivo, ultMovString, Convert.ToBase64String(foto)) });
            }
        }

        private void LecturaCartaPorteElectronica(string dispositivo, int ctg)
        {
            var puesto = repositorio.ObtenerPuestoDeTrabajoPorDispositivo(dispositivo);
            servicioComandos.Ejecutar(new NotificarLecturaCPE { PuestoId = puesto.Id, CentroId = puesto.CentroId, NroCtg = ctg });
        }
    }
}