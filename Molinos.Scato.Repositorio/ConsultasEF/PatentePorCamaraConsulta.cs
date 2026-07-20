using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class PatentePorCamaraConsulta : IConsulta<PatentePorCamaraDto>
    {
        private readonly DateTime fechaDesde;
        private readonly DateTime fechaHasta;
        private readonly int? puestoDeTrabajoId;

        public PatentePorCamaraConsulta(FiltroDashboardCardlessDto filtro)
        {
            fechaDesde = filtro.FechaDesde;
            fechaHasta = filtro.FechaHasta;
            puestoDeTrabajoId = filtro.PuestoDeTrabajoId;
        }

        public List<PatentePorCamaraDto> Ejecutar(DbContext contexto)
        {
            const string sql = @"
                SELECT
                    d.CodigoCamara,
                    COUNT(*)                                                         AS CantidadDetectadas,
                    CAST(
                        COUNT(*) * 100.0 / NULLIF(SUM(COUNT(*)) OVER (), 0)
                    AS DECIMAL(5,1))                                                 AS Porcentaje
                FROM LogIdentificacionVehicularDetalle d
                INNER JOIN LogIdentificacionVehicular l ON d.LogIdentificacionVehicular_Id = l.Id
                WHERE d.Exitoso   = 1
                  AND d.Patente IS NOT NULL
                  AND d.Intentos   = (
                        SELECT MIN(d2.Intentos)
                        FROM   LogIdentificacionVehicularDetalle d2
                        WHERE  d2.LogIdentificacionVehicular_Id = d.LogIdentificacionVehicular_Id
                          AND  d2.Exitoso = 1
                  )
                  AND l.FechaEvento >= @FechaDesde
                  AND l.FechaEvento <  DATEADD(DAY, 1, @FechaHasta)
                  AND (@PuestoDeTrabajoId IS NULL OR l.PuestoDeTrabajo_Id = @PuestoDeTrabajoId)
                GROUP BY d.CodigoCamara
                ORDER BY CantidadDetectadas DESC;";

            return contexto.Database.SqlQuery<PatentePorCamaraDto>(
                sql,
                new SqlParameter("@FechaDesde", fechaDesde.Date),
                new SqlParameter("@FechaHasta", fechaHasta.Date),
                new SqlParameter("@PuestoDeTrabajoId", (object)puestoDeTrabajoId ?? DBNull.Value)
            ).ToList();
        }
    }
}
