using System;
using System.Collections.Generic;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Ninject.Extensions.Logging;
using WebGrease.Css.Extensions;

namespace Molinos.Scato.Web.Seguridad
{
    public class ScatoClaimsAuthenticationManager : ClaimsAuthenticationManager
    {
        private readonly ILogger log;

        private ScatoClaimsAuthenticationManager()
        {
            var loggerFactory = DependencyResolver.Current.GetService<ILoggerFactory>();
            log = loggerFactory.GetCurrentClassLogger();
        }

        private static IServicioRepositorio ServicioRepositorio
        {
            get { return DependencyResolver.Current.GetService<IServicioRepositorio>(); }
        }
        private static IServicioComandos ServicioComandos
        {
            get { return DependencyResolver.Current.GetService<IServicioComandos>(); }
        }

        public override ClaimsPrincipal Authenticate(string resourceName, ClaimsPrincipal incomingPrincipal)
        {
            if (incomingPrincipal == null || !incomingPrincipal.Identity.IsAuthenticated)
            {
                return incomingPrincipal;
            }

            log.Debug("Recibida autenticación de usuario");

            var identity = ((ClaimsIdentity)incomingPrincipal.Identity);
            var claim = identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);
            if (claim == null)
            {
                var sb = new StringBuilder();
                identity.Claims.ForEach(x => sb.Append(x.Type + ": " + x.Value + "\n"));
                log.Error("No se encontró el nombre de usuario en los claims recibidos. No se puede continuar con la autenticación. Claims recibidos: {0}", sb);
                throw new SecurityException(string.Format("No se encontró el nombre de usuario en los claims recibidos. No se puede continuar con la autenticación. Claims recibidos: {0}", sb));
            }

            log.Debug("Claim Value: {0}", claim.Value);
            var nombreUsuario = claim.Value.Split('\\')[1];
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, nombreUsuario));

            ServicioComandos.Ejecutar(new ModificarUsuarioUltimoLogin { Usuario = nombreUsuario });

            log.Info("Agregando claims de permisos de Scato para el usuario {0}", nombreUsuario);
            var permisos = ServicioRepositorio.ListarPermisosPorUsuario(nombreUsuario);
            foreach (PermisoDto permiso in permisos)
            {
                if (permiso.Codigo != null)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, permiso.Codigo.Value.ToString()));
                }
            }

            try
            {
                var requestIP = GetUserIP();
                log.Info($"Ips detectado: {requestIP} para el usuario {nombreUsuario}");

                GetIpAddress();

                IPAddress IP = IPAddress.Parse(requestIP);

                log.Info($"Ips trace: ---1");

                IPHostEntry GetIPHost = Dns.GetHostEntry(IP);

                log.Info($"Ips trace: ---2");
                List<string> hostName = GetIPHost.HostName.ToString().Split('.').ToList();

                log.Info($"Ips trace: ---3");

                string ComputerName = hostName.First();

                log.Info($"Ips trace: ---4");
                string MachineName1 = Environment.MachineName;

                log.Info($"Ips trace: ---5");
                string MachineName2 = Dns.GetHostName();

                log.Info($"Ips trace: ---5");
                string MachineName3 = HttpContext.Current.Request.ServerVariables["REMOTE_HOST"].ToString();

                log.Info($"Ips trace: ---6");
                string MachineName4 = Environment.GetEnvironmentVariable("COMPUTERNAME");

                log.Info($"Ips trace: ---7");

                identity.AddClaim(new Claim("UserComputerName", ComputerName));
                log.Info("Nombre de pc detectada: {0} para el usuario {1}", String.Join(",", ComputerName, Dns.GetHostName(), MachineName1, MachineName2, MachineName3, MachineName4, requestIP), nombreUsuario);
            }
            catch (Exception ex)
            {
                log.Info("Nombre de pc detectada: no se pudo detectar para el usuario {0}.", nombreUsuario);

                log.Info(ex,"Error IP");
                //log.Info("Nombre de pc detectada: no se pudo detectar para el usuario {0}. Se intenta obtener la IP", nombreUsuario);
                //try
                //{
                //    var ips = (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? "");
                //    var RequestIP = ips.Split(',').Last().Trim().Split(':').First();

                //    identity.AddClaim(new Claim("UserComputerName", RequestIP));
                //}
                //catch (Exception)
                //{
                //    log.Info("Nombre de pc detectada: tampoco no se pudo detectar la IP para el usuario {0}", nombreUsuario);
                //}
            }

            var ci = new ClaimsIdentity(((ClaimsIdentity)incomingPrincipal.Identity).Claims, "Negotiate");

            var transformedPrincipal = new ClaimsPrincipal(ci);
            CreateSession(transformedPrincipal);
            return transformedPrincipal;
        }

        private void CreateSession(ClaimsPrincipal transformedPrincipal)
        {
            var sessionSecurityToken = new SessionSecurityToken(transformedPrincipal, TimeSpan.FromDays(365));
            FederatedAuthentication.SessionAuthenticationModule.WriteSessionTokenToCookie(sessionSecurityToken);
        }

        private string GetUserIP()
        {
            var ip = (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null
                  && HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != "")
                 ? HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]
                 : HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            log.Info($"Ips detectados string: {ip}");
            if (ip.Contains(","))
                ip = ip.Split(',').First();
            return ip.Trim();
        }

        private string GetIpAddress()
        {
            string ip = "";
            var userip = HttpContext.Current.Request.UserHostAddress;
            if (HttpContext.Current.Request.UserHostAddress != null)
            {
                Int64 macinfo = new Int64();
                string macSrc = macinfo.ToString("X");
                if (macSrc == "0")
                {
                    log.Info($"Ips v2 detectados string: {userip}");
                    ip = userip;
                }
            }

            return ip;
        }
    }
}