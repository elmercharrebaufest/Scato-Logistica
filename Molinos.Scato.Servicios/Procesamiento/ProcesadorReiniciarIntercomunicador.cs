using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Seguridad;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorReiniciarIntercomunicador : ProcesadorComando<ReiniciarIntercomunicador>
    {
        private const string NombreServicio = "IntercomunicadorService";
        private const int MaxIntentos = 3;
        private readonly IServicioComandos servicioComandos;

        public ProcesadorReiniciarIntercomunicador(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioComandos servicioComandos)
            : base(repositorio, conversor, log)
        {
            this.servicioComandos = servicioComandos;
        }

        public override Resultado Ejecutar(ReiniciarIntercomunicador comando)
        {
            var resultado = new ResultadoReiniciarIntercomunicador();

            if (!ValidarServidor(comando.Servidor, resultado))
            {
                Log.Error("Validación de servidor fallida. Servidor configurado: '{0}', servidor actual: '{1}'. Solicitado por: '{2}'.",
                    resultado.Servidor, Environment.MachineName, comando.NombreUsuario);
                EnviarMail(false, 0, comando, resultado.Servidor, new InvalidOperationException(resultado.Errores.Values.First()));
                return resultado;
            }

            var intentos = 0;
            Exception ultimoError = null;

            while (intentos < MaxIntentos)
            {
                intentos++;
                try
                {
                    Log.Info("Intento {0}/{1}: reiniciando el servicio '{2}' en el servidor '{3}'. Solicitado por: '{4}'.",
                        intentos, MaxIntentos, NombreServicio, resultado.Servidor, comando.NombreUsuario);

                    using (var servicio = new ServiceController(NombreServicio, resultado.Servidor))
                    {
                        if (servicio.Status != ServiceControllerStatus.Stopped)
                        {
                            servicio.Stop();
                            servicio.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                        }

                        servicio.Start();
                        servicio.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                    }

                    Log.Info("Servicio '{0}' en servidor '{1}' reiniciado correctamente en el intento {2}. Solicitado por: '{3}'.",
                        NombreServicio, resultado.Servidor, intentos, comando.NombreUsuario);
                    resultado.ReinicioExitoso = true;
                    EnviarMail(true, intentos, comando, resultado.Servidor, null);
                    return resultado;
                }
                catch (Exception e)
                {
                    ultimoError = e;
                    Log.Error(e, "Error al reiniciar el servicio '{0}' en servidor '{1}', intento {2}. Solicitado por: '{3}'.",
                        NombreServicio, resultado.Servidor, intentos, comando.NombreUsuario);

                    if (intentos < MaxIntentos)
                    {
                        Thread.Sleep(intentos * 2000);
                    }
                }
            }

            resultado.ReinicioExitoso = false;
            resultado.Error("", ultimoError?.Message ?? "Error desconocido al reiniciar el servicio.");
            EnviarMail(false, intentos, comando, resultado.Servidor, ultimoError);
            return resultado;
        }

        private bool ValidarServidor(string servidorEsperado, ResultadoReiniciarIntercomunicador resultado)
        {
            if (string.IsNullOrEmpty(servidorEsperado))
                resultado.Error("Servidor", "No se especificó el servidor destino del servicio Intercomunicador.");
            else
            {
                string hostEsperado;
                if (Uri.TryCreate(servidorEsperado, UriKind.Absolute, out Uri uri))
                    hostEsperado = uri.Host;
                else
                    hostEsperado = servidorEsperado;

                // Extraer solo el nombre de máquina (sin dominio)
                resultado.Servidor = hostEsperado.Split('.')[0];

                try
                {
                    var servicios = ServiceController.GetServices(resultado.Servidor);
                    var sc = Array.Find(servicios, s => string.Equals(s.ServiceName, NombreServicio, StringComparison.OrdinalIgnoreCase));

                    if (sc == null)
                    {
                        resultado.Error("Servidor",
                            $"No se encontró el servicio '{NombreServicio}' en el servidor '{resultado.Servidor}'.");
                    }
                    else if (sc.Status != ServiceControllerStatus.Running)
                    {
                        resultado.Error("Servidor",
                            $"El servicio '{NombreServicio}' en el servidor '{resultado.Servidor}' no está en ejecución (estado actual: {sc.Status}). No se procederá al reinicio.");
                    }
                }
                catch (Exception ex)
                {
                    resultado.Error("Servidor",
                        $"No se pudo verificar el estado del servicio '{NombreServicio}' en el servidor '{resultado.Servidor}': {ex.Message}");
                }
            }

            resultado.ReinicioExitoso = !resultado.HayErrores;
            return resultado.ReinicioExitoso;
        }

        private void EnviarMail(bool exitoso, int intentos, ReiniciarIntercomunicador comando, string servidor, Exception error)
        {
            try
            {
                var destinatarios = ObtenerDestinatarios();
                if (!destinatarios.Any())
                {
                    Log.Warn("No se encontraron destinatarios para la notificación del reinicio del servicio '{0}'.", NombreServicio);
                    return;
                }

                string asunto;
                string cuerpo;
                    
                if (exitoso)
                {
                    asunto = $"Scato Logística | Servicio '{NombreServicio}' reiniciado correctamente";
                    cuerpo = $"<p>El servicio <strong>{NombreServicio}</strong> fue reiniciado correctamente en el intento {intentos}.</p>";
                }
                else
                {
                    asunto = $"Scato Logística | ERROR al reiniciar el servicio '{NombreServicio}'";
                    cuerpo = $"<p>No se pudo reiniciar el servicio <strong>{NombreServicio}</strong>" +
                             (intentos > 0 ? $" luego de {intentos} intentos." : ".") + "</p>" +
                             $"<p>Motivo: {error?.Message}</p>";
                }

                cuerpo += $"<p>Servidor: {servidor}</p>" +
                          $"<p>Solicitado por: {comando.NombreUsuario}</p>" +
                          $"<p>Fecha y hora: {DateTime.Now:dd/MM/yyyy HH:mm:ss}</p>";

                servicioComandos.Ejecutar(new EnvioMail
                {
                    Destinatarios = destinatarios,
                    Titulo = asunto,
                    Cuerpo = cuerpo
                });
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al enviar el mail de notificación del reinicio del servicio '{0}'.", NombreServicio);
            }
        }

        private List<string> ObtenerDestinatarios()
        {
            var usuarios = Repositorio.Listar<Usuario>(
                u => u.RolesAsociados.Any(r => r.PermisosAsociados.Any(p => p.Codigo == PermisosScato.ReiniciarIntercomunicador))
                     && u.Email != null && u.Email != string.Empty);

            return usuarios.Select(u => u.Email).ToList();
        }
    }
}
