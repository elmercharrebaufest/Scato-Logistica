using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace Molinos.Scato.Repositorio.ComandosEF
{
    public class CrearLoteBiotecnologia : IComando<int>
    {
        private readonly int materialId;
        private readonly int camaraId;
        private readonly int centroId;
        private readonly string centroDescripcion;
        private readonly string nombreUsuario;
        private readonly DateTime fecha;

        public CrearLoteBiotecnologia(int materialId, int camaraId, int centroId, string centroDescripcion, string nombreUsuario, DateTime fecha)
        {
            this.materialId = materialId;
            this.camaraId = camaraId;
            this.centroId = centroId;
            this.centroDescripcion = centroDescripcion;
            this.nombreUsuario = nombreUsuario;
            this.fecha = fecha;
        }

        public int Ejecutar(DbContext contexto)
        {
            return contexto.Database.SqlQuery<int>(@"
                declare @NewSeqValue int 

                INSERT INTO LoteBiotecnologia (NombreUsuario,Camara_Id,Centro_Id,Material_Id,Fecha,FechaDesde,FechaHasta, NumeroDeLote) 
                VALUES (@nombreUsuario,@camaraId,@centroId,@materialId,@fecha,@fecha,@fecha, 'Default')
                set @NewSeqValue = scope_identity()
 
                INSERT INTO MuestraEnvioACamaraBiotecnologia( Recorrido_Id, LoteBiotecnologia_Id)
	                SELECT TOP 5000 R.Id, (select @NewSeqValue )
	                FROM Recorrido R
	                INNER JOIN Workflow W ON W.Id = R.Workflow_Id
	                LEFT JOIN Vehiculo V ON R.Vehiculo_Id = V.Id
	                LEFT JOIN CartaPorte CP ON CP.Id = V.CartaPorte_Id
                    LEFT JOIN Remito Remito ON Remito.Recorrido_Id = R.Id
	                WHERE R.Centro_Id = @centroId AND R.Material_Id = @materialId AND W.TipoDeWorkflow = 0 AND (Rechazado = 0 or V.TipoVehiculo = 1) AND
                    R.Terminado = 1 AND (R.TipoDocumentoIngreso = 1 or R.TipoDocumentoIngreso = 7) AND
	                NOT EXISTS(SELECT TOP 1 1 FROM ProveedorExcluidoIntacta PE WHERE PE.Proveedor_Id = CP.Destinatario_Id or PE.Proveedor_Id = Remito.ProveedorOrigen_Id) AND
	                NOT EXISTS(SELECT TOP 1 1 FROM MuestraEnvioACamaraBiotecnologia MEB WHERE MEB.Recorrido_Id = R.Id)
                
                UPDATE LoteBiotecnologia SET NumeroDeLote= @centroDesc + 'BIO' + replicate ('0',(6 - len(@NewSeqValue))) + convert(varchar, @NewSeqValue),
                                             FechaDesde = (SELECT FechaInicio FROM Recorrido WHERE Id = (SELECT MIN(Recorrido_Id) FROM MuestraEnvioACamaraBiotecnologia WHERE LoteBiotecnologia_Id = @NewSeqValue)),
                                             FechaHasta = (SELECT FechaInicio FROM Recorrido WHERE Id = (SELECT MAX(Recorrido_Id) FROM MuestraEnvioACamaraBiotecnologia WHERE LoteBiotecnologia_Id = @NewSeqValue))
                WHERE Id = @NewSeqValue

                select @NewSeqValue 
                ", new SqlParameter("@nombreUsuario", nombreUsuario)
                 , new SqlParameter("@camaraId", camaraId)
                 , new SqlParameter("@fecha", fecha)
                 , new SqlParameter("@materialId", materialId)
                 , new SqlParameter("@centroId", centroId)
                 , new SqlParameter("@centroDesc", centroDescripcion)
            ).FirstOrDefault();
        }
    }
}
