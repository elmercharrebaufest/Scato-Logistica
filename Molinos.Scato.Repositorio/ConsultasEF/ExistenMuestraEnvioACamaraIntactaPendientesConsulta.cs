using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;


namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ExistenMuestraEnvioACamaraIntactaPendientesConsulta : IConsultaEscalar<bool>
    {
        private readonly int materialId;
        private readonly int centroId;

        public ExistenMuestraEnvioACamaraIntactaPendientesConsulta(int materialId, int centroId)
        {
            this.materialId = materialId;
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
	                WHERE R.Centro_Id = @centroId AND R.Material_Id = @materialId AND W.TipoDeWorkflow = 0 AND (Rechazado = 0 or V.TipoVehiculo = 1) AND
                    R.Terminado = 1 AND (R.TipoDocumentoIngreso = 1 or R.TipoDocumentoIngreso = 7) AND
	                NOT EXISTS(SELECT TOP 1 1 FROM ProveedorExcluidoIntacta PE WHERE PE.Proveedor_Id = CP.Destinatario_Id or PE.Proveedor_Id = Remito.ProveedorOrigen_Id) AND
	                NOT EXISTS(SELECT TOP 1 1 FROM MuestraEnvioACamaraBiotecnologia MEB WHERE MEB.Recorrido_Id = R.Id))",
                new SqlParameter("@centroId", centroId),
                new SqlParameter("@materialId", materialId)).First() > 0;

        }
    }
}
