using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ObtenerStock : IConsultaEscalar<int>
    {
        private Dominio.Enums.TipoStockBines tipo;
        private System.DateTime fecha;
        private int id;
        private int materialId;

        public ObtenerStock(Dominio.Enums.TipoStockBines tipo, System.DateTime fecha, int id, int materialId)
        {
            this.tipo = tipo;
            this.fecha = fecha;
            this.id = id;
            this.materialId = materialId;
        }
        public int Ejecutar(DbContext contexto)
        {
            return
                contexto.Database.SqlQuery<int>(@"select SaldoInicial from [dbo].[CalcularSaldo](@fecha, @materialId, @tipo, @id)"
                , new SqlParameter("@fecha", fecha), new SqlParameter("@materialId", materialId), new SqlParameter("@tipo", tipo == TipoStockBines.Centro ? "C" : (tipo == TipoStockBines.Productor ? "P" : "V")), new SqlParameter("@id", id)).FirstOrDefault();
        }
    }
}
