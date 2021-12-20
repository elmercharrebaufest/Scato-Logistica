using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using Molinos.Scato.WebOperaciones.Helpers;
using Ninject.Extensions.Logging;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Text;
using System.Web.Mvc;
using WebGrease.Css.Extensions;

namespace Molinos.Scato.WebOperaciones.Seguridad
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

        private static IConfiguracionProvider ConfiguracionProvider
        {
            get { return DependencyResolver.Current.GetService<IConfiguracionProvider>(); }
        }

        public override ClaimsPrincipal Authenticate(string resourceName, ClaimsPrincipal incomingPrincipal)
        {
            if (incomingPrincipal != null && incomingPrincipal.Identity.IsAuthenticated)
            {
                log.Debug("Recibida autenticación de usuario");
                var identity = ((ClaimsIdentity)incomingPrincipal.Identity);
                //TODO: verificar de que tipo es esta claim en ADFS
                var claim = identity.Claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                if (claim != null)
                {
                    var nombreUsuario = claim.Value;
                    log.Info("Agregando claims de permisos de Scato para el usuario {0}", nombreUsuario);
                    var permisos = ServicioRepositorio.ListarPermisosPorUsuario(nombreUsuario);
                    foreach (PermisoDto permiso in permisos)
                    {
                        if (permiso.Codigo != null)
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Role, permiso.Codigo.Value.ToString()));
                            log.Debug("Agregando permiso {0} para el usuario {1}", permiso.Descripcion, nombreUsuario);
                        }
                    }
                    var centro = ServicioRepositorio.ObtenerCentroPorCodigoSap(ConfiguracionProvider.AppSettings["CodigoSapSanLorenzo"]);
                    incomingPrincipal.AddUpdateUserClaim("CentroDescripcion", centro.Descripcion);
                    incomingPrincipal.AddUpdateUserClaim("CentroId", centro.Id.ToString(CultureInfo.InvariantCulture));
                    incomingPrincipal.AddUpdateUserClaim("EstacionMeteorologica", centro.CodigoEstacionMeteorologica ?? "noConfigurada");

                }
                else
                {
                    var sb = new StringBuilder();
                    identity.Claims.ForEach(x => sb.Append(x.Type + ": " + x.Value + "\n"));
                    log.Error("No se encontró el nombre de usuario en los claims recibidos. No se puede continuar con la autenticación. Claims recibidos: {0}", sb);
                    throw new SecurityException(string.Format("No se encontró el nombre de usuario en los claims recibidos. No se puede continuar con la autenticación. Claims recibidos: {0}", sb));
                }
            }
            return incomingPrincipal;
        }

    }
}