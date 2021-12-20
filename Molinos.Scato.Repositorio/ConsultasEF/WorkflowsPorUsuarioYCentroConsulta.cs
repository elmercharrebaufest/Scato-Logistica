using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class WorkflowsPorUsuarioYCentroConsulta : IConsulta<WorkflowInfoDto>
    {
        private readonly string nombreUsuario;
        private readonly int centro;

        public WorkflowsPorUsuarioYCentroConsulta(string nombreUsuario, int centro)
        {
            this.nombreUsuario = nombreUsuario;
            this.centro = centro;
        }

        public List<WorkflowInfoDto> Ejecutar(DbContext contexto)
        {
            
            return contexto.Database.SqlQuery<WorkflowInfoDto>(
                @"SELECT distinct wf.Codigo, wf.Descripcion, wdef.ActividadInicial, wdef.FechaCreacion 
	            FROM WorkflowDefinicion wdef
		            INNER JOIN Workflow wf ON wf.Id = wdef.Workflow_Id
		            INNER JOIN Permiso p ON wdef.ActividadInicial = p.ActividadWorkflow
		            INNER JOIN RolPermiso rp ON p.Id = rp.Permiso_Id
		            INNER JOIN UsuarioRol ur ON rp.Rol_Id = ur.Rol_Id
		            INNER JOIN Usuario u ON ur.Usuario_Id = u.Id
	            WHERE 
		            wf.Centro_Id = @centroId 	
		            AND wf.Activo = 1
		            AND (u.NombreUsuario = @nombreUsuario OR EXISTS (SELECT 1 
																 FROM dbo.Suplencia sup 
																 INNER JOIN dbo.Usuario as supUsr ON supUsr.Id = sup.UsuarioSuplente_Id
																 WHERE supUsr.NombreUsuario = @nombreUsuario 
						                                        AND sup.FechaDesde <= @fecha 
						                                        AND sup.FechaHasta >= @fecha))  
		            AND wdef.Id = (SELECT MAX(wdef2.Id) FROM WorkflowDefinicion wdef2
						             WHERE wdef2.FechaActivacion <= @fecha 
						             AND wdef2.Workflow_Id = wdef.Workflow_Id AND wdef2.Activa = 1)
	            ORDER by wf.Descripcion",
                new SqlParameter("nombreUsuario", nombreUsuario),
                new SqlParameter("centroId", centro),
                new SqlParameter("fecha", DateTime.Now)).ToList();
        }
    }
}
