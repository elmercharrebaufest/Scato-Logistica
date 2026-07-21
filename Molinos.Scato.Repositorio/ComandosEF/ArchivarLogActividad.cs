using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Transactions;

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
            var opciones = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted };
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, opciones))
            {
                contexto.Database.ExecuteSqlCommand(
                    @"INSERT INTO LogActividadHistorico (Id, WorkflowInstanceId, Actividad, Fecha, ActividadXaml)
                      SELECT Id, WorkflowInstanceId, Actividad, Fecha, ActividadXaml
                      FROM LogActividad
                      WHERE WorkflowInstanceId = {0}",
                    this.workflowInstanceId);

                var filasEliminadas = contexto.Database.ExecuteSqlCommand(
                    @"DELETE FROM LogActividad
                      WHERE WorkflowInstanceId = {0}",
                    this.workflowInstanceId);

                scope.Complete();
                return filasEliminadas;
            }
        }
    }
}

