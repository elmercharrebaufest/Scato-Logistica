using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.ServicioHub.Client
{
    public class HubContextClient : IHubClient
    {
        private static readonly ConcurrentDictionary<string, Type> broadcasterTypesPorHub =
            new ConcurrentDictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["notificaLectura"] = typeof(Server.NotificaLecturaBroadcaster),
                ["notificarUsuario"] = typeof(Server.NotificarUsuarioBroadcaster),
            };

        private readonly string hubName;
        private readonly ILogger log;
        private readonly IHubContext hubContext;
        private readonly Type broadcasterType;

        public HubContextClient(string hubName, ILogger log)
        {
            this.hubName = hubName;
            this.log = log;
            this.hubContext = GlobalHost.ConnectionManager.GetHubContext(hubName);

            if (!broadcasterTypesPorHub.TryGetValue(hubName, out broadcasterType))
                throw new InvalidOperationException($"No hay un Broadcaster registrado para el hub '{hubName}' en HubContextClient.");
        }

        public Task Invoke(string method, params object[] args)
        {
            try
            {
                var metodo = broadcasterType.GetMethod(method, BindingFlags.Public | BindingFlags.Static);
                if (metodo == null)
                {
                    log.Warn("Método de hub no soportado por HubContextClient: {0} (hub: {1})", method, hubName);
                    return CompletedTask();
                }

                var parametros = new object[] { hubContext.Clients }.Concat(args ?? Enumerable.Empty<object>()).ToArray();
                metodo.Invoke(null, parametros);
                return CompletedTask();
            }
            catch (TargetInvocationException e) when (e.InnerException != null)
            {
                log.Error(e.InnerException, "Error al notificar '{0}' en el hub '{1}'", method, hubName);
                throw e.InnerException;
            }
            catch (Exception e)
            {
                log.Error(e, "Error al notificar '{0}' en el hub '{1}'", method, hubName);
                throw;
            }
        }

        private static Task CompletedTask()
        {
            return Task.FromResult(0);
        }
    }
}
