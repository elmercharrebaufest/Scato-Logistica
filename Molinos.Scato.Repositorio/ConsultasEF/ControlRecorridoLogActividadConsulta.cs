using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ControlRecorridoLogActividadConsulta : IConsulta<ControlRecorridoLogActividadConsultaDto>
    {
        private readonly Guid filtro;

        public ControlRecorridoLogActividadConsulta(Guid filtro)
        {
            this.filtro = filtro;

        }

        public virtual List<ControlRecorridoLogActividadConsultaDto> Ejecutar(DbContext contexto)
        {
            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            Recorrido recorrido = (from r in contexto.Set<Recorrido>() where r.InstanciaWorkflow == filtro select r).Single();
            if (recorrido.Terminado)
            {
                var resultado = (from logActividad in contexto.Set<LogActividadHistorico>()
                                 where logActividad.WorkflowInstanceId == filtro
                                 select new
                                 {
                                     Fecha = logActividad.Fecha,
                                     Actividad = logActividad.Actividad,
                                     Comentario = "",
                                     Tabla = "LogActividad",
                                     Usuario = ""
                                 }).Union(
                                    from controlRecorrido in contexto.Set<ControlRecorrido>()
                                    where controlRecorrido.WorkflowInstanceId == filtro
                                    select new
                                    {
                                        Fecha = controlRecorrido.Fecha,
                                        Actividad = controlRecorrido.Actividad,
                                        Comentario = controlRecorrido.Comentario,
                                        Tabla = "ControlRecorrido",
                                        Usuario = controlRecorrido.NombreUsuario
                                    })
                                .Select(
                                    s =>
                                    new ControlRecorridoLogActividadConsultaDto
                                    {
                                        Fecha = s.Fecha,
                                        Actividad = s.Actividad,
                                        Tabla = s.Tabla,
                                        Comentario = s.Comentario,
                                        Usuario = s.Usuario
                                    }).OrderBy(t => t.Fecha);
                return resultado.ToList();
            }
            else
            {
                var resultado = (from logActividad in contexto.Set<LogActividad>()
                                 where logActividad.WorkflowInstanceId == filtro
                                 select new
                                 {
                                     Fecha = logActividad.Fecha,
                                     Actividad = logActividad.Actividad,
                                     Comentario = "",
                                     Tabla = "LogActividad",
                                     Usuario = ""
                                 }).Union(
                                from controlRecorrido in contexto.Set<ControlRecorrido>()
                                where controlRecorrido.WorkflowInstanceId == filtro
                                select new
                                {
                                    Fecha = controlRecorrido.Fecha,
                                    Actividad = controlRecorrido.Actividad,
                                    Comentario = controlRecorrido.Comentario,
                                    Tabla = "ControlRecorrido",
                                    Usuario = controlRecorrido.NombreUsuario
                                })
                                .Select(
                                    s =>
                                    new ControlRecorridoLogActividadConsultaDto
                                    {
                                        Fecha = s.Fecha,
                                        Actividad = s.Actividad,
                                        Tabla = s.Tabla,
                                        Comentario = s.Comentario,
                                        Usuario = s.Usuario
                                    }).OrderBy(t => t.Fecha);
                return resultado.ToList();
            }



        }
    }
}
