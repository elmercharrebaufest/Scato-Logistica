using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Web.ServicioHub
{
    public class HubClient : IDisposable
    {
        private readonly string hubName;
        private readonly ILogger log;

        private IHubProxy proxy;
        private HubConnection connection;

        private readonly object connectionLock = new object();
        
        public HubClient(string hubName, ILogger log)
        {
            this.hubName = hubName;
            this.log = log;

            try
            {
                lock (connectionLock)
                {
                    Connect();
                }
            }
            catch (Exception e)
            {
                log.Error(e, "Error al conectar al hub {0}", hubName);
            }
        }
        
        public Task Invoke(string method, params object[] args)
        {
            try
            {
                return proxy.Invoke(method, args);
            }
            catch (InvalidOperationException e)
            {
                log.Warn(e, "Se perdió la conexión al hub '{0}'", hubName);
                try
                {
                    Reconnect();
                    return proxy.Invoke(method, args);
                }
                catch (Exception ex)
                {
                    log.Error(ex, "No se pudo reconectar al hub '{0}'", hubName);
                    throw;
                }
            }
        }


        private void Connect()
        {
            log.Debug("Conectando al hub {0}...", hubName);
            connection = new HubConnection(ConfigurationManager.AppSettings["UrlServicioNotificaciones"]);
            proxy = connection.CreateHubProxy(hubName);
            connection.Start().Wait();
            log.Debug("Hub {0} conectado.", hubName);
        }

        private void Reconnect()
        {
            lock (connectionLock)
            {
                if (connection.State == ConnectionState.Disconnected)
                {
                    log.Debug("Reconectando hub {0}...", hubName);
                    connection.Dispose();
                    Connect();
                }  
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (connection != null)
                {
                    connection.Dispose();
                }
            }
        }
    }
}