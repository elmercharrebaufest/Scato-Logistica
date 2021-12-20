using System;
using System.Data.Entity;
using System.Linq;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Seguridad;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class PermisoPorUsuarioyCodigo : IConsultaEscalar<bool>
    {
        private readonly string nombreUsuario;
        private readonly PermisosScato codigoPermiso;

        public PermisoPorUsuarioyCodigo(string nombreUsuario, PermisosScato codigoPermiso)
        {
            this.nombreUsuario = nombreUsuario;
            this.codigoPermiso = codigoPermiso;
        }

        private static bool FiltroPermisosPorUsuarioyCodigo(DbContext contexto, string nombreUsuario, PermisosScato codigoPermiso)
        {
            return contexto.Set<Permiso>()
                    .Where(x => x.RolesAsociados.Any(y =>
                    y.UsuariosAsociados.Any(z => z.NombreUsuario == nombreUsuario && x.Codigo == codigoPermiso ||
                            contexto.Set<Suplencia>().Any(s =>
                                            s.UsuarioSuplente.NombreUsuario == nombreUsuario &&
                                            s.UsuarioASuplantar.NombreUsuario == z.NombreUsuario &&
                                            s.FechaDesde <= DateTime.Today &&
                                            s.FechaHasta >= DateTime.Today)))).Select(x => 1).FirstOrDefault() != 0;
        }

        public virtual bool Ejecutar(DbContext contexto)
        {
            return FiltroPermisosPorUsuarioyCodigo(contexto, nombreUsuario, codigoPermiso);
        }
    }
}
