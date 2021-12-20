using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Repositorio.ComandosEF
{
    public class CrearArchivoDeMovimientos : IComando<int>
    {
        private readonly int materialId;
        private readonly TipoDeWorkflow tipoDeWorkflow;
        private readonly int centroId;
        private readonly string centroDesc;
        private readonly string nombreUsuario;
        private readonly DateTime fecha;
        private readonly string descCortaFirma;

        public CrearArchivoDeMovimientos(int materialId, TipoDeWorkflow tipoDeWorkflow, int centroId, string centroDesc, string nombreUsuario, DateTime fecha,string descCortaFirma)
        {
            this.materialId = materialId;
            this.tipoDeWorkflow = tipoDeWorkflow;
            this.centroId = centroId;
            this.centroDesc = centroDesc;
            this.nombreUsuario = nombreUsuario;
            this.fecha = fecha;
            this.descCortaFirma = descCortaFirma;
        }

        public int Ejecutar(DbContext contexto)
        {
             
            return contexto.Database.SqlQuery<int>(@"
                declare @NewSeqValue int,@NewSeqValue2 int 
                set @NewSeqValue2 = (select ISNULL((select MAX(id) - MIN(id)+2 from [ArchivoDeMovimientos] group by CAST(Fecha as DATE),TipoDeWorkflow having CAST(Fecha as DATE) = CAST(GETDATE() as DATE) and TipoDeWorkflow = @tipoDeWorkflow ),1))


                INSERT INTO ArchivoDeMovimientos (NombreUsuario,TipoDeWorkflow,Centro_Id,Material_Id,Fecha,FechaDesde,FechaHasta, NumeroDeArchivo) 
                VALUES (@nombreUsuario,@tipoDeWorkflow,@centroId,@materialId,@fecha,@fecha,@fecha, 'Default')
                set @NewSeqValue = scope_identity()
 
                INSERT INTO MovimientoDeTerceros( Recorrido_Id, ArchivoDeMovimientos_Id)
	                SELECT TOP 5000 R.Id, (select @NewSeqValue )
	                FROM Recorrido R
	                INNER JOIN Workflow W ON W.Id = R.Workflow_Id
	                WHERE R.Centro_Id = @centroId AND R.Material_Id = @materialId AND W.TipoDeWorkflow = @tipoDeWorkflow AND Rechazado = 0 AND
                    R.Terminado = 1 AND (R.TipoDocumentoIngreso = 1 or R.TipoDocumentoIngreso = 6 or R.TipoDocumentoIngreso = 7) AND
	                NOT EXISTS(SELECT TOP 1 1 FROM MovimientoDeTerceros MEB WHERE MEB.Recorrido_Id = R.Id)


                
                UPDATE ArchivoDeMovimientos SET NumeroDeArchivo= (CASE WHEN @tipoDeWorkflow = 0 THEN @descCortaFirma + '-IN-' + REPLACE(CONVERT(VARCHAR(10), GETDATE(), 103), '/', '') + '-' + replicate ('0',(4 - len(@NewSeqValue2))) + convert(varchar, @NewSeqValue2)
                                                                                                ELSE @descCortaFirma + '-' + CONVERT(VARCHAR(8),GETDATE(),4) + '-' + replicate ('0',(5 - len(@NewSeqValue2))) + convert(varchar, @NewSeqValue2) END),
                                             FechaDesde = (SELECT FechaInicio FROM Recorrido WHERE Id = (SELECT MIN(Recorrido_Id) FROM MovimientoDeTerceros WHERE ArchivoDeMovimientos_Id = @NewSeqValue)),
                                             FechaHasta = (SELECT FechaInicio FROM Recorrido WHERE Id = (SELECT MAX(Recorrido_Id) FROM MovimientoDeTerceros WHERE ArchivoDeMovimientos_Id = @NewSeqValue))
                WHERE Id = @NewSeqValue

                select @NewSeqValue 
                ", new SqlParameter("@nombreUsuario", nombreUsuario)
                 , new SqlParameter("@tipoDeWorkflow", tipoDeWorkflow)
                 , new SqlParameter("@centroDesc", centroDesc)
                 , new SqlParameter("@fecha", fecha)
                 , new SqlParameter("@materialId", materialId)
                 , new SqlParameter("@centroId", centroId)
                 , new SqlParameter("@descCortaFirma", descCortaFirma)
            ).FirstOrDefault();
        }
    }
}
