using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ResumenIntentosConsulta : IConsultaEscalar<ResumenIntentosDto>
    {
        private readonly DateTime fechaDesde;
        private readonly DateTime fechaHasta;
        private readonly int? puestoDeTrabajoId;

        public ResumenIntentosConsulta(FiltroDashboardCardlessDto filtro)
        {
            fechaDesde = filtro.FechaDesde;
            fechaHasta = filtro.FechaHasta;
            puestoDeTrabajoId = filtro.PuestoDeTrabajoId;
        }

        public ResumenIntentosDto Ejecutar(DbContext contexto)
        {
            const string sql = @"
                SELECT
                    AVG(CAST(IntentosPorEvento AS FLOAT))  AS PromedioGeneral,
                    MAX(CASE WHEN CAST(FechaEvento AS DATE) = CAST(GETDATE() AS DATE)
                             THEN IntentosPorEvento ELSE 0 END) AS MaxHoy
                FROM (
                    SELECT
                        l.FechaEvento,
                        COUNT(d.Id) AS IntentosPorEvento
                    FROM LogIdentificacionVehicular l
                    LEFT JOIN LogIdentificacionVehicularDetalle d ON d.LogIdentificacionVehicular_Id = l.Id
                    WHERE l.FechaEvento >= @FechaDesde
                      AND l.FechaEvento <  DATEADD(DAY, 1, @FechaHasta)
                      AND (@PuestoDeTrabajoId IS NULL OR l.PuestoDeTrabajo_Id = @PuestoDeTrabajoId)
                    GROUP BY l.Id, l.FechaEvento
                ) sub;";

            return contexto.Database.SqlQuery<ResumenIntentosDto>(
                sql,
                new SqlParameter("@FechaDesde", fechaDesde.Date),
                new SqlParameter("@FechaHasta", fechaHasta.Date),
                new SqlParameter("@PuestoDeTrabajoId", (object)puestoDeTrabajoId ?? DBNull.Value)
            ).FirstOrDefault() ?? new ResumenIntentosDto();
        }
    }
}
