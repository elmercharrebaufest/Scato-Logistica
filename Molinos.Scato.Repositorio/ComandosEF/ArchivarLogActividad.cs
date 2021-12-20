using System;
using System.Data.Entity;
using System.Data.SqlClient;

namespace Molinos.Scato.Repositorio.ComandosEF
{
    public class ArchivarLogActividad : IComando<int>
    {
        private readonly Guid workflowInstanceId;

        public ArchivarLogActividad(Guid workflowInstanceId)
        {
            this.workflowInstanceId = workflowInstanceId;
        }

        public int Ejecutar(DbContext contexto)
        {
            return contexto.Database.ExecuteSqlCommand(
                    @"DELETE LogActividad
                        OUTPUT
                            DELETED.Id,
                            DELETED.WorkflowInstanceId,
                            DELETED.Actividad,
                            DELETED.Fecha,
                            DELETED.ActividadXaml
                        INTO LogActividadHistorico (Id, WorkflowInstanceId, Actividad, Fecha, ActividadXaml)
                      WHERE WorkflowInstanceId = @instanceId",
                new SqlParameter("@instanceId", workflowInstanceId)
            );
        }
    }
}

