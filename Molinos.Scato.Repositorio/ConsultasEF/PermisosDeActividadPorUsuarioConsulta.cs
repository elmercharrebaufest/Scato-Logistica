using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class PermisosDeActividadPorUsuarioConsulta : IConsulta<string>
    {
        private readonly string nombreUsuario;

        public PermisosDeActividadPorUsuarioConsulta(string nombreUsuario)
        {
            this.nombreUsuario = nombreUsuario;
        }

        public List<string> Ejecutar(DbContext contexto)
        {
            var today = DateTime.Today;
            return contexto.Set<Permiso>()
                        .Where(x => x.TipoPermiso == TipoPermiso.Actividad && x.ActividadWorkflow != null && x.RolesAsociados.Any(y =>
                                        y.UsuariosAsociados.Any(z => z.NombreUsuario == nombreUsuario ||
                                                contexto.Set<Suplencia>().Any(s =>
                                                                s.UsuarioSuplente.NombreUsuario == nombreUsuario &&
                                                                s.UsuarioASuplantar.NombreUsuario == z.NombreUsuario &&
                                                                s.FechaDesde <= today &&
                                                                s.FechaHasta >= today))))
                         .Select(x => x.ActividadWorkflow).ToList();
        }
    }
}
