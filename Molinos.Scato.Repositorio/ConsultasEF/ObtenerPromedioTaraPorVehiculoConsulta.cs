using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ObtenerPromedioTaraPorVehiculoConsulta : IConsultaEscalar<PromedioTaraVehiculoDto>
    {
        private readonly int recorridoId;

        private const string SqlBase = @"
            SELECT 
                vv.Patente1 AS PatenteCamion,
                vv.Patente2 AS PatenteAcoplado,
                vv.Patente3 AS PatenteAcoplado2,
                UPPER(ISNULL(ch.Apellido, '')) AS ChoferApellido,
                UPPER(ISNULL(ch.Nombre, '')) AS ChoferNombre,
                ISNULL(ch.NumeroDeDocumento, '') AS ChoferNumeroDocumento
            FROM Recorrido r
            INNER JOIN VehiculoView vv
                ON vv.Recorrido_Id = r.Id
            LEFT JOIN Chofer ch
                ON ch.Id = r.Chofer_Id
            WHERE r.Id = @RecorridoId"; 

        private const string SqlPromedio = @"
            SELECT
                AVG(r.PesoTara)                
                AS PromedioTara
            FROM VehiculoView vv      
            INNER JOIN Recorrido r
                ON r.Id = vv.Recorrido_Id
            WHERE vv.Patente1 = @PatenteCamion
              AND ISNULL(vv.Patente2, '') = ISNULL(@PatenteAcoplado, '') 
              AND ISNULL(vv.Patente3, '') = ISNULL(@PatenteAcoplado2, '')  
              AND r.Terminado = 1
              AND r.Rechazado = 0 ";

        public ObtenerPromedioTaraPorVehiculoConsulta(int recorridoId)
        {
            this.recorridoId = recorridoId;
        }

        public PromedioTaraVehiculoDto Ejecutar(DbContext contexto)
        {
            var baseData = contexto.Database
                .SqlQuery<PromedioTaraVehiculoDto>(
                    SqlBase,
                    new SqlParameter("@RecorridoId", recorridoId))
                .FirstOrDefault();

            if (baseData == null)
            {
                return new PromedioTaraVehiculoDto { PromedioTara = 0 };
            }

            var promedio = contexto.Database
                .SqlQuery<int>(
                    SqlPromedio,
                    new SqlParameter("@PatenteCamion", baseData.PatenteCamion ?? string.Empty),
                    new SqlParameter("@PatenteAcoplado", baseData.PatenteAcoplado ?? string.Empty),
                    new SqlParameter("@PatenteAcoplado2", baseData.PatenteAcoplado2 ?? string.Empty))
                .FirstOrDefault();

            baseData.PromedioTara = promedio;
            return baseData;
        }
    }
}