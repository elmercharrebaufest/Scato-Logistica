using System;
using System.ServiceModel;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Impl;
using Ninject;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Seguridad
{
    public class AuthorizationManager : ServiceAuthorizationManager
    {
        public static IKernel KernelInstance { private get; set; }
        private readonly ILogger log;

        public AuthorizationManager()
        {
            log = KernelInstance.Get<ILoggerFactory>().GetCurrentClassLogger();
        }

        private IRepositorio Repositorio
        {
            get { return KernelInstance.Get<IRepositorio>(); }
        }

        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            bool autorizado;
            try
            {
                var usuarioRequest = operationContext.ServiceSecurityContext.WindowsIdentity;
                log.Debug("Autorizando usuario {0}", usuarioRequest.Name);
                var usuarioApp = System.Security.Principal.WindowsIdentity.GetCurrent();
                // Verifica si es otro sitio de Scato
                log.Debug("Usuario Request {0} - Usuario App {1}", usuarioRequest.Name, usuarioApp.Name);
                autorizado = usuarioRequest.Name == usuarioApp.Name;
                // Verifica si es un usuario de pa aplicacion
                if (!autorizado && operationContext.Host.Description.ServiceType == typeof(ServicioWorkflows))
                {
                    if (usuarioRequest.Name != null && usuarioRequest.Name.Contains("\\"))
                    {
                        var usuario = usuarioRequest.Name.Split('\\')[1];
                        autorizado = Repositorio.Existe<Usuario>(u => u.NombreUsuario == usuario);
                    }
                }
                log.Debug("Usuario {0} autorizado={1}", usuarioRequest.Name, autorizado);
            }
            catch (Exception e)
            {
                log.Error(e, "Ocurrió un error durante la autorización");
                autorizado = false;
            }
            return autorizado;
        }

    }
}
