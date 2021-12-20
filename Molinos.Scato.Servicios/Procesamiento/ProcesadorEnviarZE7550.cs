using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.ServiciosSap;
using Molinos.Scato.Servicios.Urenport;
using Ninject.Extensions.Logging;
using PdfiumViewer;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.ServiceModel;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEnviarZE7550 : ProcesadorComando<EnviarZE7550>
    {
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioRepositorio servicioRepositorio;
        private readonly IConfiguracionProvider configuracion;
        private readonly ZSDWS_SCATO servicioSap;

        public ProcesadorEnviarZE7550(IRepositorio repositorio, IServicioComandos servicioComandos, IConversor conversor, ILogger log, ZSDWS_SCATO servicioSap, IConfiguracionProvider configuracion, IServicioRepositorio servicioRepositorio)
            : base(repositorio, conversor, log)
        {
            this.servicioComandos = servicioComandos;
            this.configuracion = configuracion;
            this.servicioRepositorio = servicioRepositorio;
            this.servicioSap = servicioSap;
        }

        public override Resultado Ejecutar(EnviarZE7550 comando)
        {
            var resultado = new ResultadoEnviarZE7550();

            var listaZE = Repositorio.Listar<ZE7550TransmisionASap>(x => x.Estado == EstadoTransmisionASap.Pendiente);
            resultado.TransaccionesTotales = listaZE.Count;
            foreach (var ze7550 in listaZE)
            {
                var transmision = Conversor.Convertir<ZE7550TransmisionASap, Z_SDMF_RFC_ZE7550>(ze7550);

                if (CupoReingresado(ze7550.NumCarPor, ze7550.Centro))
                {
                    resultado.CuposReenviados += 1;
                    ze7550.Estado = EstadoTransmisionASap.Cancelada;
                    Log.Info("Se actualiza el estado de la transmisión a: {0}. El cupo fue utilizado en otro recorrido.", ze7550.Estado);
                    servicioComandos.Ejecutar(new ActualizarZE7550TransmisionASap { Dto = ze7550 });
                }
                else
                {
                    if (PasoTiempoMaximoDeRechazo(ze7550.InstanciaWorkflow))
                    {
                        TransmitirPendienteASap(transmision, ze7550, resultado);
                    }
                    else
                    {
                        resultado.CuposNoPaso24Horas += 1;
                    }
                }

            }
            return resultado;
        }

        private void TransmitirPendienteASap(Z_SDMF_RFC_ZE7550 transmision, ZE7550TransmisionASap ze7550, ResultadoEnviarZE7550 resultado)
        {
            try
            {
                var request = new Z_SDMF_RFC_ZE7550Request
                {
                    Z_SDMF_RFC_ZE7550 = transmision
                };
                Log.Info("Se Va a enviar la request Z_SDMF_RFC_ZE7550Request");
                Log.Debug(transmision.ToXml());
                var respuesta = servicioSap.Z_SDMF_RFC_ZE7550(request);
                if (respuesta.Z_SDMF_RFC_ZE7550Response.EX_RESULTADO.Any(res => res.TIPO == "E"))
                {
                    Log.Warn("Respuesta con errores en la ejecución de la función ZE7550");
                    ze7550.MensajeError =
                        respuesta.Z_SDMF_RFC_ZE7550Response.EX_RESULTADO.First(res => res.TIPO == "E").TEXTO;
                    ze7550.Estado = EstadoTransmisionASap.Error;
                    Log.Warn("Error: {0}", ze7550.MensajeError);
                    //resultado.Error("Respuesta con errores en la ejecución de la función ZE7550", ze7550.MensajeError);
                    resultado.TransaccionesEnviadasError += 1;
                }
                else
                {
                    Log.Info("Respuesta Correcta en la ejecución de la función ZE7550");
                    ze7550.MensajeError = "";
                    ze7550.Estado = EstadoTransmisionASap.Correcto;
                    resultado.TransaccionesEnviadasOK += 1;
                }
                Log.Info("Se actualiza el estado de la transmisión a: {0}", ze7550.Estado);
                servicioComandos.Ejecutar(new ActualizarZE7550TransmisionASap { Dto = ze7550 });
            }
            catch (Exception e)
            {
                resultado.TransaccionesEnviadasError += 1;
                Log.Warn(e, "Error en la ejecución de la función ZE7550");
                try
                {
                    ze7550.Estado = EstadoTransmisionASap.Pendiente;
                    ze7550.MensajeError = e.Message.Length > 50 ? e.Message.Substring(0, 50) : e.Message;
                    //resultado.Error("Error en la ejecución de la función ZE7550", ze7550.MensajeError);
                    Log.Info("Se actualiza el estado de la transmisión a: {0}", ze7550.Estado);
                    servicioComandos.Ejecutar(new ActualizarZE7550TransmisionASap { Dto = ze7550 });
                }
                catch (Exception e2)
                {
                    Log.Error(e2, "No se pudo actualizar el estado de la transmisión");
                    //resultado.Error("Error", "No se pudo actualizar el estado de la transmisión");
                }
            }
        }

        private bool CupoReingresado(string cartaPorte, string codigoCentroSap)
        {
            return Repositorio.Existe<Recorrido>(x => x.Terminado && !x.Rechazado && x.NumeroDocumentoIngreso == cartaPorte && x.Centro.CodigoSAP == codigoCentroSap);
        }

        private bool PasoTiempoMaximoDeRechazo(Guid instanceId)
        {
            double minutos = 0;
            double.TryParse(System.Configuration.ConfigurationManager.AppSettings["DemoraTransmisionASAPZE7550"], out minutos);
            var fechaEgreso = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == instanceId).FechaEgreso;
            return DateTime.Now.Subtract(fechaEgreso.HasValue ? fechaEgreso.Value : DateTime.Now).TotalMinutes > minutos;
        }
    }
}