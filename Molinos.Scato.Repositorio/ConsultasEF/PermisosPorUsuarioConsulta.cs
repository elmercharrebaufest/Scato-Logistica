using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class PermisosPorUsuarioConsulta : IConsulta<Permiso>
    {
        private readonly string nombreUsuario;

        public PermisosPorUsuarioConsulta(string nombreUsuario)
        {
            this.nombreUsuario = nombreUsuario;
        }

        public List<Permiso> Ejecutar(DbContext contexto)
        {
            var today = DateTime.Today;
            return contexto.Set<Permiso>()
                .Where(x => x.RolesAsociados.Any(y =>
                                y.UsuariosAsociados.Any(z => z.NombreUsuario == nombreUsuario ||
                                        contexto.Set<Suplencia>().Any(s =>
                                                        s.UsuarioSuplente.NombreUsuario == nombreUsuario &&
                                                        s.UsuarioASuplantar.NombreUsuario == z.NombreUsuario &&
                                                        s.FechaDesde <= today &&
                                                        s.FechaHasta >= today)))).ToList();
        }
    }
}
