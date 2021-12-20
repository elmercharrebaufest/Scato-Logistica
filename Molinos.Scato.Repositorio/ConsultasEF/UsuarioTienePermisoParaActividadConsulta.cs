using System;
using System.Data.Entity;
using System.Linq;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class UsuarioTienePermisoParaActividadConsulta : IConsultaEscalar<bool>
    {
        private readonly string nombreUsuario;
        private readonly string actividad;

        public UsuarioTienePermisoParaActividadConsulta(string nombreUsuario, string actividad)
        {
            this.nombreUsuario = nombreUsuario;
            this.actividad = actividad;
        }

        public bool Ejecutar(DbContext contexto)
        {
            var today = DateTime.Today;
            return contexto.Set<Permiso>()
                        .Where(x => x.TipoPermiso == TipoPermiso.Actividad && x.ActividadWorkflow == actividad && x.RolesAsociados.Any(y =>
                                        y.UsuariosAsociados.Any(z => z.NombreUsuario == nombreUsuario ||
                                                contexto.Set<Suplencia>().Any(s =>
                                                                s.UsuarioSuplente.NombreUsuario == nombreUsuario &&
                                                                s.UsuarioASuplantar.NombreUsuario == z.NombreUsuario &&
                                                                s.FechaDesde <= today &&
                                                                s.FechaHasta >= today))))
                         .Select(x => 1).FirstOrDefault() != 0;
        }
    }
}
