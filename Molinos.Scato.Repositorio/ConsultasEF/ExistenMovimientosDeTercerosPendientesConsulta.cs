using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using Molinos.Scato.Dominio.Enums;


namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ExistenMovimientosDeTercerosPendientesConsulta : IConsultaEscalar<bool>
    {
        private readonly int materialId;
        private readonly TipoDeWorkflow tipoDeWorkflow;
        private readonly int centroId;

        public ExistenMovimientosDeTercerosPendientesConsulta(int materialId, TipoDeWorkflow tipoDeWorkflow, int centroId)
        {
            this.materialId = materialId;
            this.tipoDeWorkflow = tipoDeWorkflow;
            this.centroId = centroId;
        }

        public bool Ejecutar(DbContext contexto)
        {
            ((IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;


            return contexto.Database.SqlQuery<int>(
                @"(SELECT COUNT(*)
	                FROM Recorrido R
	                INNER JOIN Workflow W ON W.Id = R.Workflow_Id
	                LEFT JOIN Vehiculo V ON R.Vehiculo_Id = V.Id
	                LEFT JOIN CartaPorte CP ON CP.Id = V.CartaPorte_Id
                    LEFT JOIN Remito Remito ON Remito.Recorrido_Id = R.Id
	                WHERE R.Centro_Id = @centroId AND R.Material_Id = @materialId AND W.TipoDeWorkflow = @tipoDeWorkflow AND Rechazado = 0 AND
                    R.Terminado = 1 AND (R.TipoDocumentoIngreso = 1 or R.TipoDocumentoIngreso = 6 or R.TipoDocumentoIngreso = 7) AND
	                NOT EXISTS(SELECT TOP 1 1 FROM MovimientoDeTerceros MEB WHERE MEB.Recorrido_Id = R.Id))",
                new SqlParameter("@centroId", centroId),
                new SqlParameter("@materialId", materialId),
                new SqlParameter("@tipoDeWorkflow", tipoDeWorkflow)).First() > 0;

        }
    }
}
